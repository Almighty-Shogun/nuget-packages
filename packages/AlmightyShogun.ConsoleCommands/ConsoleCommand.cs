namespace AlmightyShogun.ConsoleCommands;

/// <summary>
/// Describes a discovered console command.
/// </summary>
///
/// <author>Almighty-Shogun</author>
/// <since>1.0.0</since>
public sealed class ConsoleCommand
{
    /// <summary>
    /// Gets the primary command name.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>1.0.0</since>
    public string Name { get; }

    /// <summary>
    /// Gets the command description, if any.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>1.0.0</since>
    public string? Description { get; }

    /// <summary>
    /// Gets the command aliases.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>1.0.0</since>
    public IReadOnlyList<string> Aliases { get; }

    /// <summary>
    /// Gets the command usage text.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>1.0.0</since>
    public string Usage { get; }

    /// <summary>
    /// Gets the example invocation, if one is declared.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>1.0.0</since>
    public string? Example { get; }

    /// <summary>
    /// Creates console command metadata.
    /// </summary>
    ///
    /// <param name="name">The primary command name.</param>
    /// <param name="description">The command description, if any.</param>
    /// <param name="aliases">The command aliases.</param>
    /// <param name="usage">The usage text without the command name.</param>
    /// <param name="example">The example arguments without the command name, if any.</param>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>1.0.0</since>
    internal ConsoleCommand(string name, string? description, IReadOnlyList<string> aliases, string usage, string? example)
    {
        Name = name;
        Description = description;
        Aliases = aliases;
        Usage = string.IsNullOrWhiteSpace(usage) ? name : $"{name} {usage}";
        Example = string.IsNullOrWhiteSpace(example) ? null : $"{name} {example}";
    }
}
