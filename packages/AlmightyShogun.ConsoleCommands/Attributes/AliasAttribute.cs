namespace AlmightyShogun.ConsoleCommands;

/// <summary>
/// Declares extra names the dispatcher accepts for a command, so a short form or a retired name keeps working without a
/// second command class. Every alias resolves to the same class as the command name itself.
/// </summary>
///
/// <param name="aliases">
/// The names to accept, matched case-insensitively like the command name. An alias already taken by another command is
/// dropped with a warning, so the first registration keeps it.
/// </param>
///
/// <author>Almighty-Shogun</author>
/// <since>1.0.0</since>
[AttributeUsage(AttributeTargets.Class, Inherited = false)]
public sealed class AliasAttribute(params string[] aliases) : Attribute
{
    /// <summary>
    /// Gets the declared alternative names, in the order they were written. Empty when the attribute was applied with no
    /// arguments, which claims no extra names and leaves the command reachable only by its own.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>1.0.0</since>
    public IReadOnlyList<string> Aliases { get; } = aliases;
}
