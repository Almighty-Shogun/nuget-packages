namespace AlmightyShogun.ConsoleCommands;

/// <summary>
/// Describes a registered console command.
/// </summary>
///
/// <param name="Name">The primary command name.</param>
/// <param name="Aliases"> The command aliases. </param>
/// <param name="ImplementationType">
/// The command implementation type.
/// </param>
///
/// <author>Almighty-Shogun</author>
/// <since>4.0.0</since>
internal sealed record ConsoleCommandDescriptor(string Name, IReadOnlyList<string> Aliases, Type ImplementationType);
