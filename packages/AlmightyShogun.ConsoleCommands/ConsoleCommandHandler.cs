using Microsoft.Extensions.Logging;

namespace AlmightyShogun.ConsoleCommands;

/// <summary>
/// Runs the console input loop and dispatches user input to registered console commands.
/// </summary>
///
/// <author>Almighty-Shogun</author>
/// <since>1.0.0</since>
internal class ConsoleCommandHandler : IConsoleCommandHandler
{
    /// <summary>
    /// Stores the cancellation source used to stop the active command input loop.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    private CancellationTokenSource? _stopSource;

    /// <summary>
    /// Stores the logger used for lifecycle and command dispatch errors.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>1.0.0</since>
    private readonly ILogger<ConsoleCommandHandler> _logger;

    /// <summary>
    /// Stores command names and aliases mapped to their executable command implementations.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>1.0.0</since>
    private readonly Dictionary<string, IInternalConsoleCommand> _commands;

    /// <summary>
    /// Creates a console command handler from registered command services.
    /// </summary>
    ///
    /// <param name="logger">The logger used to report lifecycle and dispatch failures.</param>
    /// <param name="commands">The command services registered in dependency injection.</param>
    ///
    /// <exception cref="InvalidOperationException">Thrown when a registered command does not use the package command base implementation.</exception>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>1.0.0</since>
    public ConsoleCommandHandler(ILogger<ConsoleCommandHandler> logger, IEnumerable<IConsoleCommand> commands)
    {
        _logger = logger;
        _commands = new Dictionary<string, IInternalConsoleCommand>();

        foreach (IConsoleCommand consoleCommand in commands)
        {
            if (consoleCommand is not IInternalConsoleCommand internalCommand)
            {
                throw new InvalidOperationException($"{consoleCommand.GetType().Name} must inherit {nameof(ConsoleCommandBase)}.");
            }

            _commands.Add(consoleCommand.Name.ToLowerInvariant(), internalCommand);

            foreach (string alias in consoleCommand.Aliases)
            {
                if (string.IsNullOrWhiteSpace(alias)) continue;

                _commands.TryAdd(alias.ToLowerInvariant(), internalCommand);
            }
        }
    }

    /// <inheritdoc />
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>1.1.0</since>
    public async Task StartAsync(CancellationToken cancellationToken = default)
    {
        Console.TreatControlCAsInput = false;

        if (_stopSource is not null)
        {
            _logger.LogError("Cannot start the console command handler because it is already running.");

            return;
        }

        _stopSource = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
        CancellationToken stopToken = _stopSource.Token;

        try
        {
            while (!stopToken.IsCancellationRequested)
            {
                string? input;

                try
                {
                    input = await Console.In.ReadLineAsync(stopToken);
                }
                catch (OperationCanceledException)
                {
                    break;
                }

                if (!string.IsNullOrWhiteSpace(input))
                {
                    await HandleCommandAsync(input);
                }
            }
        }
        catch (Exception exception)
        {
            _logger.LogError(exception, "The console command handler stopped unexpectedly.");
        }
        finally
        {
            _stopSource?.Dispose();
            _stopSource = null;
        }
    }

    /// <inheritdoc />
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    public void Stop()
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

    /// <summary>
    /// Parses a console input line and dispatches it to the matching registered command.
    /// </summary>
    ///
    /// <param name="input">The raw console input containing a command name followed by optional arguments.</param>
    ///
    /// <returns>A task that represents the asynchronous dispatch operation.</returns>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>1.0.0</since>
    private async Task HandleCommandAsync(string input)
    {
        ConsoleUtils.RemoveLastLine();

        string[] parts = input.Split(' ');
        string commandName = parts[0];

        if (_commands.TryGetValue(commandName, out IInternalConsoleCommand? command))
        {
            await command.InternallyExecuteCommandAsync(parts.Skip(1).ToArray());
        }
        else
        {
            if (_logger.IsEnabled(LogLevel.Warning))
            {
                _logger.LogWarning("{CommandName:y} is not registered as a console command", commandName);
            }
        }
    }
}
