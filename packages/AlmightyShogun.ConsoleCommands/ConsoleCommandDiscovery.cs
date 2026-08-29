using System.Reflection;
using AlmightyShogun.Utils;

namespace AlmightyShogun.ConsoleCommands;

/// <summary>
/// Finds console command classes and builds the public metadata a help listing is rendered from.
/// </summary>
///
/// <author>Almighty-Shogun</author>
/// <since>1.0.0</since>
public static class ConsoleCommandDiscovery
{
    /// <summary>
    /// Retrieves metadata for the commands declared in the calling assembly, which is what a help command in the startup
    /// project wants. Reach for the overload taking assemblies when the commands live elsewhere.
    /// </summary>
    ///
    /// <returns>The metadata for each command class, in declaration order.</returns>
    ///
    /// <exception cref="InvalidOperationException">
    /// A discovered class breaks one of the command rules. Reported rather than skipped, so a malformed command is the
    /// same failure here as it is at registration.
    /// </exception>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    public static IReadOnlyList<ConsoleCommand> GetAllCommands() => GetAllCommands([Assembly.GetCallingAssembly()]);

    /// <summary>
    /// Retrieves metadata for the commands declared in the given assemblies, read from the class attributes and the
    /// parameters of each handler method rather than from resolved instances, so nothing is constructed.
    /// </summary>
    ///
    /// <param name="assemblies">
    /// The assemblies to scan, in the order they should be searched. An empty array yields nothing; the overload taking no
    /// assembly at all is the one that falls back to the calling assembly.
    /// </param>
    ///
    /// <returns>The metadata for each command class, in assembly then declaration order.</returns>
    ///
    /// <exception cref="InvalidOperationException">
    /// A discovered class breaks one of the command rules. Reported rather than skipped, so a malformed command is the
    /// same failure here as it is at registration.
    /// </exception>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>1.0.0</since>
    public static IReadOnlyList<ConsoleCommand> GetAllCommands(Assembly[] assemblies)
        => [.. GetConsoleCommandTypes(assemblies).Select(Describe)];

    /// <summary>
    /// Retrieves every command type in the given assemblies, valid or not, leaving each caller to decide what a malformed
    /// one means.
    /// </summary>
    ///
    /// <param name="assemblies">The assemblies to scan, in the order they should be searched.</param>
    ///
    /// <returns>The concrete types assignable to <see cref="IConsoleCommand"/>, lazily.</returns>
    ///
    /// <remarks>
    /// Nothing is filtered here. A silent filter is what previously let a class carrying the attribute disappear from the
    /// help listing while registration reported it as an error, which meant the two disagreed about what a command is.
    /// </remarks>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>3.0.0</since>
    internal static IEnumerable<Type> GetConsoleCommandTypes(Assembly[] assemblies)
        => TypeDiscovery.FindAssignableTypes<IConsoleCommand>(assemblies);

    /// <summary>
    /// Builds the public metadata for one command type.
    /// </summary>
    ///
    /// <param name="commandType">The command class to reflect over.</param>
    ///
    /// <returns>
    /// The metadata, whose usage string lists each handler parameter as <c>&lt;name:Type&gt;</c>. A trailing
    /// <see cref="CancellationToken"/> is left out, because the dispatcher supplies it rather than the user typing it.
    /// </returns>
    ///
    /// <exception cref="InvalidOperationException">The class breaks one of the command rules.</exception>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    internal static ConsoleCommand Describe(Type commandType)
    {
        (ConsoleCommandAttribute attribute, MethodInfo handlerMethod) = CommandMetadata.Describe(commandType);

        string usage = string.Join(" ", handlerMethod.GetParameters()
            .Where(parameter => parameter.ParameterType != typeof(CancellationToken))
            .Select(parameter => $"<{parameter.Name}:{parameter.ParameterType.Name}>"));

        return new ConsoleCommand(
            attribute.Name,
            attribute.Description,
            commandType.GetCustomAttribute<AliasAttribute>()?.Aliases ?? [],
            usage,
            commandType.GetCustomAttribute<ExampleAttribute>()?.Example
        );
    }
}
