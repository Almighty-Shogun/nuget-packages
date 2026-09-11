namespace AlmightyShogun.RemoteCommands;

/// <summary>
/// Describes a registered remote command.
/// </summary>
///
/// <param name="Name">The command name.</param>
/// <param name="ImplementationType">The command implementation type.</param>
///
/// <author>Almighty-Shogun</author>
/// <since>4.0.0</since>
internal sealed record RemoteCommandDescriptor(string Name, Type ImplementationType);
