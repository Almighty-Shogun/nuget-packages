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
    /// <since>Unreleased</since>
    private readonly Lock _lifecycleGate = new();

    /// <summary>
    /// The source cancelled to end the running loop, and the flag for whether one is running at all: it is non-null only
    /// between the start of a loop and its exit.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
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
    /// <since>Unreleased</since>
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

        try
        {
            while (!stopSource.IsCancellationRequested)
            {
                string? input;

                try
                {
                    input = await Console.In.ReadLineAsync(stopSource.Token);
                }
                catch (OperationCanceledException)
                {
                    break;
                }

                if (string.IsNullOrWhiteSpace(input)) continue;

                Utils.ConsoleUtils.RemoveLastLine();

                await HandleCommandAsync(input, stopSource.Token);
            }
        }
        catch (Exception exception)
        {
            _logger.LogError(exception, "The console command handler stopped unexpectedly.");
        }
        finally
        {
            lock (_lifecycleGate)
            {
                _stopSource?.Dispose();
                _stopSource = null;
            }
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
    /// Claims one name for a command, first come first served. A clash is a warning rather than a throw, so one careless
    /// alias cannot stop an application from starting.
    /// </summary>
    ///
    /// <param name="name">The name or alias to claim. A blank one is ignored, since it could never be typed.</param>
    /// <param name="commandType">The type the name should dispatch to.</param>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
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
    /// <returns>A task that completes when the command has finished, or immediately when no command matched.</returns>
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
            {
                _logger.LogWarning("{CommandName:y} is not registered as a console command", commandName);
            }
            else
            {
                _logger.LogWarning(
                    "{CommandName:y} is not registered as a console command. Did you mean {Suggestion:c}?",
                    commandName, suggestion
                );
            }

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
    /// The nearest name, or <c>null</c> when none is within one edit per three characters. That threshold keeps a short
    /// name from suggesting an unrelated one of similar length.
    /// </returns>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
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
    /// <since>Unreleased</since>
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
