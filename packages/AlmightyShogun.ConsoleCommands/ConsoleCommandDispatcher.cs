using System.Reflection;
using System.Text;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace AlmightyShogun.ConsoleCommands;

internal sealed class ConsoleCommandDispatcher
{
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
    
    private readonly Action<string, Exception> _commandFailed;

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
                commandType.Name);
        }
    }


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
                    costs[i - 1, j - 1] + substitution);
            }
        }

        return costs[left.Length, right.Length];
    }
}
