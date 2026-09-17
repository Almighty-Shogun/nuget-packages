namespace AlmightyShogun.ConsoleCommands;

/// <summary>
/// Declares extra names the dispatcher accepts for a command, so a short form or a retired name keeps working without a
/// second command class. Every alias resolves to the same class as the command name itself.
/// </summary>
/// <author>Almighty-Shogun</author>
/// <since>1.0.0</since>
[AttributeUsage(AttributeTargets.Class, Inherited = false)]
public sealed class AliasAttribute : Attribute
{
    /// <summary>
    /// Declares extra names the dispatcher accepts for a command, so a short form or a retired name keeps working without a
    /// second command class. Every alias resolves to the same class as the command name itself.
    /// </summary>
    ///
    /// <param name="aliases">The names to accept, matched case-insensitively like the command name.</param>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>1.0.0</since>
    public AliasAttribute(params string[] aliases)
    {
        ArgumentNullException.ThrowIfNull(aliases);

        var seen = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

        foreach (string alias in aliases)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(alias);

            if (alias.Any(char.IsWhiteSpace))
                throw new ArgumentException("Alias must not contain whitespace.", nameof(aliases));

            if (!seen.Add(alias))
                throw new ArgumentException($"Alias '{alias}' is declared more than once.", nameof(aliases));
        }

        Aliases = aliases;
    }

    /// <summary>
    /// The declared alternative names, in the order they were written. Empty when the attribute was applied with no
    /// arguments, which claims no extra names and leaves the command reachable only by its own.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>1.0.0</since>
    public IReadOnlyList<string> Aliases { get; }
}
