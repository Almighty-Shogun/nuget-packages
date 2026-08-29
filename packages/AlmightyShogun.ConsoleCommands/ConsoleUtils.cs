using System.Reflection;
using AlmightyShogun.Utils;

namespace AlmightyShogun.ConsoleCommands;

/// <summary>
/// Discovers console command types and builds the public metadata a help listing is rendered from. Console input and
/// cursor helpers live on <see cref="AlmightyShogun.Utils.ConsoleUtils"/>.
/// </summary>
///
/// <author>Almighty-Shogun</author>
/// <since>1.0.0</since>
public static class ConsoleUtils
{
    /// <summary>
    /// Retrieves metadata for the commands declared in the calling assembly, which is what a help command in the startup
    /// project wants. Reach for the overload taking assemblies when the commands live elsewhere.
    /// </summary>
    ///
    /// <returns>
    /// The metadata for each valid command class, in declaration order. A class carrying the attribute but failing the
    /// handler-method rules is skipped rather than reported, so this never throws on a malformed command.
    /// </returns>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    public static IReadOnlyList<ConsoleCommand> GetAllCommands()
        => GetAllCommands([Assembly.GetCallingAssembly()]);

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
    /// <returns>
    /// The metadata for each valid command class, in assembly then declaration order. A class carrying the attribute but
    /// failing the handler-method rules is skipped rather than reported, so this never throws on a malformed command.
    /// </returns>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>1.0.0</since>
    public static IReadOnlyList<ConsoleCommand> GetAllCommands(Assembly[] assemblies)
        => [.. GetConsoleCommandTypes(assemblies).Select(Describe)];

    /// <summary>
    /// Retrieves the command types in the given assemblies that pass every rule the dispatcher relies on.
    /// </summary>
    ///
    /// <param name="assemblies">The assemblies to scan, in the order they should be searched.</param>
    ///
    /// <returns>
    /// The concrete command types that carry <see cref="ConsoleCommandAttribute"/> and expose exactly one public
    /// <c>ExecuteAsync</c> method returning <see cref="Task"/>, lazily.
    /// </returns>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>3.0.0</since>
    internal static IEnumerable<Type> GetConsoleCommandTypes(Assembly[] assemblies)
        => TypeDiscovery.FindAssignableTypes<IConsoleCommand>(assemblies)
            .Where(type => CommandMetadata.TryDescribe(type, out _, out _, out _));

    /// <summary>
    /// Builds the public metadata for one command type that has already passed <see cref="CommandMetadata.TryDescribe"/>.
    /// </summary>
    ///
    /// <param name="commandType">The command class to reflect over, already known to satisfy every rule.</param>
    ///
    /// <returns>
    /// The metadata, whose usage string lists each handler parameter as <c>&lt;name:Type&gt;</c>. A trailing
    /// <see cref="CancellationToken"/> is left out, because the dispatcher supplies it rather than the user typing it.
    /// </returns>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    internal static ConsoleCommand Describe(Type commandType)
    {
        CommandMetadata.TryDescribe(commandType, out ConsoleCommandAttribute attribute, out MethodInfo handlerMethod, out _);

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
