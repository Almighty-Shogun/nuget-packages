namespace AlmightyShogun.ConsoleCommands;

/// <summary>
/// Defines console command metadata used during discovery and registration.
/// </summary>
///
/// <author>Almighty-Shogun</author>
/// <since>1.0.0</since>
internal interface IConsoleCommand
{
    /// <summary>
    /// Gets the primary command name.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>1.0.0</since>
    string Name { get; }

    /// <summary>
    /// Gets the command description, if any.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>1.0.0</since>
    string? Description { get; }

    /// <summary>
    /// Gets the command aliases.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>1.0.0</since>
    IReadOnlyList<string> Aliases { get; }
}
