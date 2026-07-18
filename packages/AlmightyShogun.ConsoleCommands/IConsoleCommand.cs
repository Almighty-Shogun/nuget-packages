namespace AlmightyShogun.ConsoleCommands;

/// <summary>
/// Exposes command metadata required by the internal command dispatcher.
/// </summary>
///
/// <author>Almighty-Shogun</author>
/// <since>1.0.0</since>
internal interface IConsoleCommand
{
    /// <summary>
    /// Gets the command name used for dispatch.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>1.0.0</since>
    string Name { get; }

    /// <summary>
    /// Gets the optional command description.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>1.0.0</since>
    string? Description { get; }

    /// <summary>
    /// Gets aliases that should dispatch to the same command.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>1.0.0</since>
    IReadOnlyList<string> Aliases { get; }
}
