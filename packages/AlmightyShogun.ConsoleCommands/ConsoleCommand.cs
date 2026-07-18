namespace AlmightyShogun.ConsoleCommands;

/// <summary>
/// Represents metadata for a discovered console command.
/// </summary>
///
/// <param name="name">The command name.</param>
/// <param name="description">The optional command description.</param>
/// <param name="aliases">Aliases that invoke the command.</param>
/// <param name="usage">Generated usage text for the command arguments.</param>
/// <param name="example">Optional example arguments for the command.</param>
///
/// <author>Almighty-Shogun</author>
/// <since>1.0.0</since>
public sealed class ConsoleCommand(string name, string? description, string[] aliases, string usage, string? example)
{
    /// <summary>
    /// Gets the command name.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>1.0.0</since>
    public string Name { get; } = name;

    /// <summary>
    /// Gets the optional command description.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>1.0.0</since>
    public string? Description { get; } = description;

    /// <summary>
    /// Gets aliases that invoke the command.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>1.0.0</since>
    public string[] Aliases { get; } = aliases;

    /// <summary>
    /// Gets the generated usage text for the command.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>1.0.0</since>
    public string? Usage { get; } = $"{name} {usage}";

    /// <summary>
    /// Gets the optional example text for the command.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>1.0.0</since>
    public string? Example { get; } = string.IsNullOrWhiteSpace(example) ? null : $"{name} {example}";
}
