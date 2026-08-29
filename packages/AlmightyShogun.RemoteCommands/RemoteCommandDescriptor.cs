namespace AlmightyShogun.RemoteCommands;

/// <summary>
/// Names a registered command and the class implementing it, so the listener can hold a dispatch table without holding a
/// command instance. Built once during registration and never read from the wire.
/// </summary>
///
/// <param name="Name">
/// The wire name from <see cref="RemoteCommandAttribute"/>, matched with ordinal case sensitivity against the request's
/// <c>command</c> field.
/// </param>
/// <param name="ImplementationType">
/// The command class, resolved from a fresh scope for each request. Registered as a transient service under this exact
/// type, so resolving it never returns a shared instance.
/// </param>
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
