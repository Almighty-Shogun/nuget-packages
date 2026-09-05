namespace AlmightyShogun.RemoteCommands;

/// <summary>
/// Names a registered command and the class implementing it, so the listener can hold a dispatch table without holding a
/// command instance. Built once during registration and never read from the wire.
/// </summary>
///
/// <param name="Name">The wire name, read from the command class's <see cref="RemoteCommandAttribute"/>.</param>
/// <param name="ImplementationType">The command class.</param>
///
/// <remarks>
/// This exists so the singleton listener never captures a command. Reading the name from the attribute rather than from a
/// constructed command is what makes the dispatch table buildable without resolving anything, which in turn is what lets a
/// command depend on scoped services.
/// </remarks>
///
/// <author>Almighty-Shogun</author>
/// <since>Unreleased</since>
internal sealed record RemoteCommandDescriptor(string Name, Type ImplementationType);
