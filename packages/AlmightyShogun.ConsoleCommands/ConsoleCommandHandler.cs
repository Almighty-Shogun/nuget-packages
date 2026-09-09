using System.Threading.Channels;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;

namespace AlmightyShogun.ConsoleCommands;

/// <summary>
/// Reads the console a line at a time and dispatches each line to the command registered under its first token. Commands
/// are resolved per invocation from a fresh scope, so one may depend on scoped application services.
/// </summary>
///
/// <author>Almighty-Shogun</author>
/// <since>1.0.0</since>
internal sealed class ConsoleCommandHandler : IConsoleCommandHandler
{
    /// <summary>
    /// Guards the lifecycle source, so <see cref="Stop"/> cannot observe it between the null check and the cancel.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>4.0.0</since>
    private readonly Lock _lifecycleGate = new();

    /// <summary>
    /// The source canceled to end the running loop, and the flag for whether one is running at all. It is set before the
    /// setup the loop needs, the reader thread included, and cleared in the loop's <c>finally</c>, so a failure in that
    /// setup leaves it set with no loop running, after which every further <see cref="StartAsync"/> logs that one is
    /// already running and returns.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>4.0.0</since>
    private CancellationTokenSource? _stopSource;

    /// <summary>
    /// The logger every complaint goes through, both the dispatcher's own and the argument errors each command reports,
    /// since a command is handed this one rather than holding a logger of its own.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>1.0.0</since>
    private readonly ILogger<ConsoleCommandHandler> _logger;

    /// <summary>
    /// The factory for the per-invocation scope. A command is transient, but its dependencies may be scoped, which is what
    /// makes the scope rather than the root provider necessary.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>4.0.0</since>
    private readonly IServiceScopeFactory _scopeFactory;

    /// <summary>
    /// Every name and alias mapped to its command type, compared case-insensitively so the prompt is forgiving about
    /// capitalisation. Types rather than instances, since each invocation builds its own.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>1.0.0</since>
    private readonly Dictionary<string, Type> _commands = new(StringComparer.OrdinalIgnoreCase);

    /// <summary>
    /// Builds the dispatch table from the registered descriptors, without resolving a command. A malformed one has already
    /// been rejected at registration, so the only rule left to check here is that the class can actually be executed.
    /// </summary>
    ///
    /// <param name="logger">
    /// The logger every complaint goes through, and the one handed to each command so it can report unusable arguments
    /// without depending on a logger itself.
    /// </param>
    /// <param name="scopeFactory">The factory used to build a scope per invocation.</param>
    /// <param name="descriptors">
    /// Every registered command's name, aliases and class, enumerated once to build the name table. Descriptors rather
    /// than commands, so this singleton never captures an instance and a command stays free to depend on scoped services.
    /// </param>
    ///
    /// <exception cref="InvalidOperationException">
    /// Thrown when a registered command does not derive from <see cref="ConsoleCommandBase"/>, and so has no execution
    /// entry point the dispatcher can call.
    /// </exception>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>1.0.0</since>
    public ConsoleCommandHandler(
        ILogger<ConsoleCommandHandler> logger,
        IServiceScopeFactory scopeFactory,
        IEnumerable<ConsoleCommandDescriptor> descriptors
    )
    {
        _logger = logger;
        _scopeFactory = scopeFactory;

        foreach (ConsoleCommandDescriptor descriptor in descriptors)
        {
            if (!typeof(IInternalConsoleCommand).IsAssignableFrom(descriptor.ImplementationType))
                throw new InvalidOperationException($"{descriptor.ImplementationType.Name} must inherit {nameof(ConsoleCommandBase)}.");

            Register(descriptor.Name, descriptor.ImplementationType);

            foreach (string alias in descriptor.Aliases)
                Register(alias, descriptor.ImplementationType);
        }
    }

    /// <inheritdoc />
    public event EventHandler<ConsoleCommandErrorEvent>? CommandFailed;

    /// <inheritdoc />
    public async Task StartAsync(CancellationToken cancellationToken = default)
    {
        CancellationTokenSource stopSource;

        lock (_lifecycleGate)
        {
            if (_stopSource is not null)
            {
                _logger.LogError("Cannot start the console command handler because it is already running.");

                return;
            }

            stopSource = _stopSource = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
        }

        try
        {
            Console.TreatControlCAsInput = false;
        }
        catch (IOException) { }

        Channel<string> lines = Channel.CreateUnbounded<string>(new UnboundedChannelOptions
        {
            SingleReader = true,
            SingleWriter = true
        });

        var requests = new SemaphoreSlim(0, 1);
        var readerStop = new CancellationTokenSource();

        var reader = new Thread(() => ReadLines(lines.Writer, requests, readerStop.Token))
        {
            IsBackground = true,
            Name = "Console command reader"
        };

        reader.Start();

        try
        {
            while (!stopSource.IsCancellationRequested)
            {
                string input;

                requests.Release();

                try
                {
                    input = await lines.Reader.ReadAsync(stopSource.Token);
                }
                catch (OperationCanceledException) when (stopSource.IsCancellationRequested)
                {
                    break;
                }
                catch (ChannelClosedException exception) when (exception.InnerException is null)
                {
                    break;
                }

                if (string.IsNullOrWhiteSpace(input)) continue;

                Utils.ConsoleUtils.RemoveLastLine();

                await HandleCommandAsync(input, stopSource.Token);
            }
        }
        catch (OperationCanceledException) when (stopSource.IsCancellationRequested) { }
        catch (Exception exception)
        {
            _logger.LogError(exception, "The console command handler stopped unexpectedly.");
        }
        finally
        {
            lines.Writer.TryComplete();

            await readerStop.CancelAsync();
            readerStop.Dispose();

            lock (_lifecycleGate)
            {
                _stopSource?.Dispose();
                _stopSource = null;
            }
        }
    }

    /// <summary>
    /// Reads standard input on a thread of its own, one line for each one the loop asks for, so that a read already under way
    /// is abandoned rather than waited out when the handler stops and nothing is taken off the input between two requests.
    /// </summary>
    ///
    /// <param name="writer">
    /// The channel end the lines are published to. Reaching the end of the input stream completes it, which is how the loop
    /// learns there will be no further line.
    /// </param>
    /// <param name="requests">
    /// Released once by the loop for every line it wants. Nothing is read until one has been taken, which is what leaves
    /// standard input to a command for as long as one is running.
    /// </param>
    /// <param name="cancellationToken">
    /// Signaled by the loop on its way out, ending the wait for the next request. That wait is the only point this can be
    /// stopped at, since a read already under way has to finish first.
    /// </param>
    ///
    /// <remarks>
    /// This runs on a background thread, because a read parked in <see cref="TextReader.ReadLine"/> cannot be interrupted
    /// and a foreground thread sitting in one would hold the process open. A stop that interrupts a read leaves the thread
    /// to outlive <see cref="StartAsync"/> by however long the next line or the end of the stream takes to arrive: the loop
    /// completes the channel on its way out, so the line that finally arrives is refused by the channel, dropped, and the
    /// thread ends.
    ///
    /// A read that fails while no stop has been asked for completes the channel with the exception rather than as an
    /// ordinary end of input, so it reaches the loop and is reported as an unexpected stop. It arrives wrapped in a
    /// <see cref="ChannelClosedException"/>, except for an <see cref="OperationCanceledException"/>, which completes the
    /// channel as a cancellation and reaches the loop as one. That is why the loop ends quietly on a cancellation only once
    /// a stop has actually been asked for. A cancellation raised once the stop is under way, by the wait for the next
    /// request or by the read itself, completes the channel as an ordinary end of input instead.
    /// </remarks>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    private static void ReadLines(ChannelWriter<string> writer, SemaphoreSlim requests, CancellationToken cancellationToken)
    {
        try
        {
            while (true)
            {
                requests.Wait(cancellationToken);

                if (Console.In.ReadLine() is not { } line)
                {
                    writer.TryComplete();

                    return;
                }

                if (!writer.TryWrite(line)) return;
            }
        }
        catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
        {
            writer.TryComplete();
        }
        catch (Exception exception)
        {
            writer.TryComplete(exception);
        }
    }

    /// <inheritdoc />
    public void Stop()
    {
        lock (_lifecycleGate)
        {
            if (_stopSource is null)
            {
                _logger.LogError("Cannot stop the console command handler because it is not running.");

                return;
            }

            try
            {
                _stopSource.Cancel();
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, "Failed to stop the console command handler.");
            }
        }
    }

    /// <summary>
    /// Claims one name for a command, first come, first served. A clash is a warning rather than a throw, so one careless
    /// alias cannot stop an application from starting.
    /// </summary>
    ///
    /// <param name="name">The name or alias to claim. A blank one is ignored, since it could never be typed.</param>
    /// <param name="commandType">The type the name should dispatch to.</param>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>4.0.0</since>
    private void Register(string name, Type commandType)
    {
        if (string.IsNullOrWhiteSpace(name)) return;

        if (!_commands.TryAdd(name, commandType) && _logger.IsEnabled(LogLevel.Warning))
        {
            _logger.LogWarning(
                "{Name:y} is already registered by {Existing:c}, so {Skipped:c} will never be invocable under that name",
                name,
                _commands[name].Name,
                commandType.Name
            );
        }
    }

    /// <summary>
    /// Splits one input line and runs the command it names in its own scope. An unknown name is reported with the closest
    /// registered match rather than a bare failure.
    /// </summary>
    ///
    /// <param name="input">The line as typed, guaranteed non-blank by the caller so the first token always exists.</param>
    /// <param name="cancellationToken">Signalled when the handler is stopping, forwarded to a command that accepts one.</param>
    ///
    /// <returns>
    /// A task that completes when the command has finished, or immediately when no command matched. A failure inside the
    /// command is logged and raises <see cref="CommandFailed"/> rather than faulting the task; the exceptions below are the
    /// ones that fault it instead.
    /// </returns>
    ///
    /// <exception cref="OperationCanceledException">
    /// Thrown out of the command while <paramref name="cancellationToken"/> is already signaled. The filter tests that
    /// token alone, so one raised for an unrelated token during shutdown is rethrown with it. Either way it escapes into
    /// <see cref="StartAsync"/>, which ends the loop on it without logging, since a stop had already been asked for.
    /// </exception>
    /// <exception cref="InvalidOperationException">
    /// The command class could not be resolved from the scope, which is what a constructor dependency of its own that was
    /// never registered produces. Resolution happens before the <c>try</c>, so this is neither logged nor turned into
    /// <see cref="CommandFailed"/>.
    /// </exception>
    /// <exception cref="Exception">
    /// Whatever the command's own constructor threw, for the same reason, or whatever a <see cref="CommandFailed"/>
    /// subscriber threw, since the event is raised from inside the <c>catch</c> that would otherwise have handled it.
    /// </exception>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>1.0.0</since>
    private async Task HandleCommandAsync(string input, CancellationToken cancellationToken)
    {
        string[] parts = input.Split(' ', StringSplitOptions.RemoveEmptyEntries);
        string commandName = parts[0];

        if (!_commands.TryGetValue(commandName, out Type? commandType))
        {
            if (!_logger.IsEnabled(LogLevel.Warning)) return;

            string? suggestion = FindClosestCommand(commandName);

            if (suggestion is null)
                _logger.LogWarning("{CommandName:y} is not registered as a console command", commandName);
            else
                _logger.LogWarning(
                    "{CommandName:y} is not registered as a console command. Did you mean {Suggestion:c}?",
                    commandName, suggestion
                );

            return;
        }

        await using AsyncServiceScope scope = _scopeFactory.CreateAsyncScope();

        var command = (IInternalConsoleCommand)scope.ServiceProvider.GetRequiredService(commandType);

        try
        {
            await command.InternallyExecuteCommandAsync(parts[1..], _logger, cancellationToken);
        }
        catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
        {
            throw;
        }
        catch (Exception exception)
        {
            _logger.LogError(exception, "The {CommandName:y} console command failed", commandName);

            CommandFailed?.Invoke(this, new ConsoleCommandErrorEvent(commandName, exception));
        }
    }

    /// <summary>
    /// Picks the registered name a mistyped one most likely meant.
    /// </summary>
    ///
    /// <param name="commandName">The name that matched nothing.</param>
    ///
    /// <returns>
    /// The nearest registered name, or <c>null</c> when no name is within one edit per three characters of
    /// <paramref name="commandName"/>. The allowance never drops below one, so a name shorter than three characters still
    /// suggests anything a single edit away, and the ratio only governs names of three characters or more.
    /// </returns>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>4.0.0</since>
    private string? FindClosestCommand(string commandName)
    {
        string? best = null;
        var bestDistance = int.MaxValue;

        foreach (string candidate in _commands.Keys)
        {
            int distance = Distance(commandName, candidate);

            if (distance >= bestDistance) continue;

            bestDistance = distance;
            best = candidate;
        }

        return bestDistance <= Math.Max(1, commandName.Length / 3) ? best : null;
    }

    /// <summary>
    /// Computes the Levenshtein distance between two names, comparing case-insensitively so capitalisation alone never
    /// counts as a difference.
    /// </summary>
    ///
    /// <param name="left">The name that was typed.</param>
    /// <param name="right">The registered name to measure it against.</param>
    ///
    /// <returns>The number of insertions, deletions, and substitutions separating the two.</returns>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>4.0.0</since>
    private static int Distance(string left, string right)
    {
        var costs = new int[left.Length + 1, right.Length + 1];

        for (var i = 0; i <= left.Length; i++)
            costs[i, 0] = i;

        for (var j = 0; j <= right.Length; j++)
            costs[0, j] = j;

        for (var i = 1; i <= left.Length; i++)
        {
            for (var j = 1; j <= right.Length; j++)
            {
                int substitution = char.ToLowerInvariant(left[i - 1]) == char.ToLowerInvariant(right[j - 1]) ? 0 : 1;

                costs[i, j] = Math.Min(
                    Math.Min(costs[i - 1, j] + 1, costs[i, j - 1] + 1),
                    costs[i - 1, j - 1] + substitution
                );
            }
        }

        return costs[left.Length, right.Length];
    }
}
