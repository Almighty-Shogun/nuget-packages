namespace AlmightyShogun.ConsoleCommands;

/// <summary>
/// Declares alternative names for a console command.
/// </summary>
/// <author>Almighty-Shogun</author>
/// <since>1.0.0</since>
[AttributeUsage(AttributeTargets.Class, Inherited = false)]
public sealed class AliasAttribute : Attribute
{
    /// <summary>
    /// Creates an alias declaration for a console command.
    /// </summary>
    ///
    /// <param name="aliases">The aliases to declare.</param>
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
    /// Gets the declared aliases.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>1.0.0</since>
    public IReadOnlyList<string> Aliases { get; }
}
