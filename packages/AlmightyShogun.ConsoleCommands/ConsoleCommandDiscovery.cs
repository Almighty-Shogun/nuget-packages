using System.Reflection;
using AlmightyShogun.Utils;

namespace AlmightyShogun.ConsoleCommands;

/// <summary>
/// Discovers console commands and exposes their metadata.
/// </summary>
///
/// <author>Almighty-Shogun</author>
/// <since>1.0.0</since>
public static class ConsoleCommandDiscovery
{
    /// <summary>
    /// Retrieves console command metadata from the calling assembly.
    /// </summary>
    ///
    /// <returns>The discovered console commands.</returns>
    ///
    /// <exception cref="InvalidOperationException">
    /// Thrown when a discovered command is invalid.
    /// </exception>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>4.0.0</since>
    public static IReadOnlyList<ConsoleCommand> GetAllCommands() => GetAllCommands([Assembly.GetCallingAssembly()]);

    /// <summary>
    /// Retrieves console command metadata from the specified assemblies.
    /// </summary>
    ///
    /// <param name="assemblies">The assemblies to scan.</param>
    ///
    /// <returns>The discovered console commands.</returns>
    ///
    /// <exception cref="InvalidOperationException">
    /// Thrown when a discovered command is invalid.
    /// </exception>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>1.0.0</since>
    public static IReadOnlyList<ConsoleCommand> GetAllCommands(Assembly[] assemblies)
        => [.. GetConsoleCommandTypes(assemblies).Select(Describe)];

    /// <summary>
    /// Finds console command implementation types in the specified assemblies.
    /// </summary>
    ///
    /// <param name="assemblies">The assemblies to scan.</param>
    ///
    /// <returns>The discovered command types.</returns>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>3.0.0</since>
    internal static IEnumerable<Type> GetConsoleCommandTypes(Assembly[] assemblies)
        => TypeDiscovery.FindAssignableTypes<IConsoleCommand>(assemblies);

    /// <summary>
    /// Builds public metadata for a console command type.
    /// </summary>
    ///
    /// <param name="commandType">The command type to describe.</param>
    ///
    /// <returns>
    /// The console command metadata.
    /// </returns>
    ///
    /// <exception cref="InvalidOperationException">Thrown when the command metadata or example cannot be represented.</exception>
    ///
    /// <remarks>
    /// Only a trailing token is dropped, matching the one position <see cref="ConsoleCommandBase"/> fills in. A token
    /// declared anywhere else is counted as a parameter the user supplies and the usage string says so, but the binder has
    /// no conversion from a typed token to a <see cref="CancellationToken"/>. A line long enough to reach that position is
    /// rejected by the conversion and a shorter one by the argument count, so unless the parameter is optional the command
    /// can never be run at all.
    ///
    /// The array tail is recognised after the token has been dropped, so a handler ending in an array followed by a
    /// <see cref="CancellationToken"/> is rendered as the variadic one it binds as.
    /// </remarks>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>4.0.0</since>
    internal static ConsoleCommand Describe(Type commandType)
    {
        (ConsoleCommandAttribute attribute, MethodInfo handlerMethod) = CommandMetadata.Describe(commandType);

        ParameterInfo[] parameters = handlerMethod.GetParameters();

        if (parameters.Length > 0 && parameters[^1].ParameterType == typeof(CancellationToken))
            parameters = parameters[..^1];

        bool hasVariadicTail = CommandArgumentBinder.HasVariadicTail(parameters);

        string usage = string.Join(
            " ",
            parameters.Select((parameter, index) => hasVariadicTail && index == parameters.Length - 1
                ? $"<{parameter.Name}:{parameter.ParameterType.GetElementType()!.Name}...>"
                : $"<{parameter.Name}:{parameter.ParameterType.Name}>")
        );

        ExampleAttribute? exampleAttribute =
            commandType.GetCustomAttribute<ExampleAttribute>();

        string? example = exampleAttribute is null
            ? null
            : FormatExample(
                exampleAttribute.Arguments,
                attribute.ArgumentParsing);
        
        return new ConsoleCommand(
            attribute.Name,
            attribute.Description,
            commandType.GetCustomAttribute<AliasAttribute>()?.Aliases ?? [],
            usage,
            example
        );
    }


    /// <summary>
    /// Formats example arguments according to the configured parsing mode.
    /// </summary>
    ///
    /// <param name="arguments">The individual example arguments.</param>
    /// <param name="parsingMode">The parsing mode used by the command.</param>
    ///
    /// <returns>The formatted example.</returns>
    ///
    /// <exception cref="InvalidOperationException">
    /// Thrown when the parsing mode is unknown or the example cannot be represented by it.
    /// </exception>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    private static string FormatExample(
        string[] arguments,
        ArgumentParsingMode parsingMode)
    {
        return parsingMode switch
        {
            ArgumentParsingMode.Spaces =>
                FormatSpaceExample(arguments),

            ArgumentParsingMode.Quotes =>
                FormatQuotedExample(arguments),

            _ => throw new InvalidOperationException(
                $"Unknown argument parsing mode: {parsingMode}")
        };
    }
    
    /// <summary>
    /// Formats an example for space-delimited argument parsing.
    /// </summary>
    ///
    /// <exception cref="InvalidOperationException">
    /// Thrown when an argument contains whitespace.
    /// </exception>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    private static string FormatSpaceExample(string[] arguments)
    {
        if (arguments.Any(static argument =>
                argument.Any(char.IsWhiteSpace)))
        {
            throw new InvalidOperationException(
                "Example arguments cannot contain whitespace when space parsing is used.");
        }

        return string.Join(" ", arguments);
    }
    
    /// <summary>
    /// Formats an example for quote-aware argument parsing.
    /// </summary>
    ///
    /// <exception cref="InvalidOperationException">
    /// Thrown when an argument contains a quote that cannot be represented by the parser.
    /// </exception>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    private static string FormatQuotedExample(string[] arguments)
    {
        if (arguments.Any(static argument => argument.Contains('"')))
        {
            throw new InvalidOperationException(
                "Example arguments containing quotes cannot be represented by quote parsing.");
        }

        return string.Join(
            " ",
            arguments.Select(static argument =>
                argument.Length == 0 ||
                argument.Any(char.IsWhiteSpace)
                    ? $"\"{argument}\""
                    : argument));
    }
}
