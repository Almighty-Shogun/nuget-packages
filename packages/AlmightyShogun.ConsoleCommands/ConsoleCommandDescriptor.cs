namespace AlmightyShogun.ConsoleCommands;

/// <summary>
/// Names a registered command and the class serving it, so the dispatcher can build its name table without constructing
/// anything. Built once during registration and never read from console input.
/// </summary>
///
/// <param name="Name">The name from <see cref="ConsoleCommandAttribute"/>, already checked to be typeable.</param>
/// <param name="Aliases">
/// The alternative names from <see cref="AliasAttribute"/>, empty when the class declares none. Each claims a slot in the
/// same table as <paramref name="Name"/>, so an alias colliding with a real name is reported the same way.
/// </param>
/// <param name="ImplementationType">
/// The command class, resolved from a fresh scope for each invocation. Registered as a transient service under this exact
/// type, so resolving it never returns a shared instance.
/// </param>
///
/// <remarks>
/// This exists so the singleton dispatcher never captures a command. Reading the names from the attributes rather than
/// from constructed commands is what makes the table buildable without resolving anything, which in turn is what lets a
/// command depend on scoped services.
/// </remarks>
///
/// <author>Almighty-Shogun</author>
/// <since>Unreleased</since>
internal sealed record ConsoleCommandDescriptor(string Name, IReadOnlyList<string> Aliases, Type ImplementationType);
