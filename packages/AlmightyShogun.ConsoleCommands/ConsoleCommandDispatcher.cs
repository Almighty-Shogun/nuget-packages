using System.Reflection;
using System.Text;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace AlmightyShogun.ConsoleCommands;

/// <summary>
/// Resolves and executes registered console commands from input lines.
/// </summary>
///
/// <author>Almighty-Shogun</author>
/// <since>Unreleased</since>
internal sealed class ConsoleCommandDispatcher
{
    /// <summary>
    /// The logger used for command lookup, execution, and argument errors.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    private readonly ILogger<ConsoleCommandHandler> _logger;

    /// <summary>
    /// Creates the scope used for each command invocation.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    private readonly IServiceScopeFactory _scopeFactory;

    /// <summary>
    /// Maps command names and aliases to their implementation types.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    private readonly Dictionary<string, Type> _commands = new(StringComparer.OrdinalIgnoreCase);
    
    /// <summary>
    /// Reports command execution failures back to the owning handler.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    private readonly Action<string, Exception> _commandFailed;

    /// <summary>
    /// Creates a command dispatcher from the registered command descriptors.
    /// </summary>
    ///
    /// <param name="logger">The logger used during command dispatch.</param>
    /// <param name="scopeFactory">The factory used to create command invocation scopes.</param>
    /// <param name="descriptors">The registered console command descriptors.</param>
    /// <param name="commandFailed">The callback invoked when command execution fails.</param>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    internal ConsoleCommandDispatcher(
        ILogger<ConsoleCommandHandler> logger,
        IServiceScopeFactory scopeFactory,
        IEnumerable<ConsoleCommandDescriptor> descriptors,
        Action<string, Exception> commandFailed)
    {
        _logger = logger;
        _scopeFactory = scopeFactory;
        _commandFailed = commandFailed;

        foreach (ConsoleCommandDescriptor descriptor in descriptors)
        {
            if (!typeof(IInternalConsoleCommand).IsAssignableFrom(descriptor.ImplementationType))
                throw new InvalidOperationException($"{descriptor.ImplementationType.Name} must inherit {nameof(ConsoleCommandBase)}.");

            Register(descriptor.Name, descriptor.ImplementationType);

            foreach (string alias in descriptor.Aliases)
                Register(alias, descriptor.ImplementationType);
        }
    }

    /// <summary>
    /// Dispatches an input line to the matching console command.
    /// </summary>
    ///
    /// <param name="input">The non-empty input line to dispatch.</param>
    /// <param name="cancellationToken">The cancellation token forwarded to the command.</param>
    ///
    /// <returns>A task that completes when command execution finishes.</returns>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>1.0.0</since>
    internal async Task DispatchAsync(string input, CancellationToken cancellationToken)
    {
        int separatorIndex = input.IndexOf(' ');
        string commandName = separatorIndex < 0
            ? input
            : input[..separatorIndex];

        string argumentText = separatorIndex < 0
            ? string.Empty
            : input[(separatorIndex + 1)..];

        if (!_commands.TryGetValue(commandName, out Type? commandType))
        {
            if (!_logger.IsEnabled(LogLevel.Warning)) return;
            string? suggestion = FindClosestCommand(commandName);

            if (suggestion is null)
                _logger.LogWarning("{CommandName:y} is not registered as a console command", commandName);
            else
                _logger.LogWarning(
                    "{CommandName:y} is not registered as a console command. Did you mean {Suggestion:c}?",
                    commandName,
                    suggestion);

            return;
        }

        ConsoleCommandAttribute attribute =
            commandType.GetCustomAttribute<ConsoleCommandAttribute>()
            ?? throw new InvalidOperationException($"{commandType.Name} is missing ConsoleCommandAttribute.");

        string[] arguments = attribute.ArgumentParsing switch
        {
            ArgumentParsingMode.Spaces =>
                argumentText.Split(' ', StringSplitOptions.RemoveEmptyEntries),

            ArgumentParsingMode.Quotes =>
                TokenizeQuoted(argumentText),

            _ => throw new InvalidOperationException($"Unknown argument parsing mode: {attribute.ArgumentParsing}")
        };

        try
        {
            await using AsyncServiceScope scope = _scopeFactory.CreateAsyncScope();

            var command = (IInternalConsoleCommand)scope.ServiceProvider.GetRequiredService(commandType);

            await command.InternallyExecuteCommandAsync(arguments, _logger, cancellationToken);
        }
        catch (OperationCanceledException exception) when (exception.CancellationToken == cancellationToken &&
                                                           cancellationToken.IsCancellationRequested)
        {
            throw;
        }
        catch (Exception exception)
        {
            _logger.LogError(exception, "The {CommandName:y} console command failed", commandName);
            _commandFailed(commandName, exception);
        }
    }


    /// <summary>
    /// Registers a command name or alias in the dispatch table.
    /// </summary>
    ///
    /// <param name="name">The command name or alias.</param>
    /// <param name="commandType">The command implementation type.</param>
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
                commandType.Name);
        }
    }


    /// <summary>
    /// Splits quote-aware command arguments while preserving whitespace inside quoted values.
    /// </summary>
    ///
    /// <param name="input">The raw argument text.</param>
    ///
    /// <returns>The parsed argument tokens.</returns>
    ///
    /// <exception cref="FormatException">
    /// Thrown when a quoted argument is not terminated.
    /// </exception>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    private static string[] TokenizeQuoted(string input)
    {
        var tokens = new List<string>();
        var current = new StringBuilder();

        var inQuotes = false;
        var tokenStarted = false;

        foreach (char c in input)
        {
            if (c is '"')
            {
                inQuotes = !inQuotes;
                tokenStarted = true;
                continue;
            }

            if (char.IsWhiteSpace(c) && !inQuotes)
            {
                if (!tokenStarted)
                    continue;

                tokens.Add(current.ToString());
                current.Clear();
                tokenStarted = false;
                continue;
            }

            current.Append(c);
            tokenStarted = true;
        }

        if (inQuotes)
            throw new FormatException("Unterminated quoted argument.");

        if (tokenStarted)
            tokens.Add(current.ToString());

        return [.. tokens];
    }

    /// <summary>
    /// Finds the closest registered command name to the supplied input.
    /// </summary>
    ///
    /// <param name="commandName">The unmatched command name.</param>
    ///
    /// <returns>The closest acceptable match, or <c>null</c> when none is close enough.</returns>
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
    /// Calculates the case-insensitive Levenshtein distance between two strings.
    /// </summary>
    ///
    /// <param name="left">The first string.</param>
    /// <param name="right">The second string.</param>
    ///
    /// <returns>The Levenshtein distance.</returns>
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
                    costs[i - 1, j - 1] + substitution);
            }
        }

        return costs[left.Length, right.Length];
    }
}
