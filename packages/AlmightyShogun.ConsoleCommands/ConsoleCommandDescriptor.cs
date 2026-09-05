namespace AlmightyShogun.ConsoleCommands;

/// <summary>
/// Names a registered command and the class serving it, so the dispatcher can build its name table without constructing
/// anything. Built once during registration and never read from console input.
/// </summary>
///
/// <param name="Name">The name from <see cref="ConsoleCommandAttribute"/>, already checked to be typeable.</param>
/// <param name="Aliases">
/// The alternative names from <see cref="AliasAttribute"/>, empty when the class declares none.
/// </param>
/// <param name="ImplementationType">
/// The command class, registered by <see cref="ConsoleCommandExtensions"/> and resolved by that same type.
/// </param>
///
/// <remarks>
/// This exists so the singleton dispatcher never captures a command. Reading the names from the attributes rather than
/// from constructed commands is what makes the table buildable without resolving anything, which in turn is what lets a
/// command depend on scoped services.
/// </remarks>
///
/// <author>Almighty-Shogun</author>
/// <since>4.0.0</since>
internal sealed record ConsoleCommandDescriptor(string Name, IReadOnlyList<string> Aliases, Type ImplementationType);
