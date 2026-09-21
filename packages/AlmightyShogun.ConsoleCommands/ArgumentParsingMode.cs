namespace AlmightyShogun.ConsoleCommands;

/// <summary>
/// Specifies how console command arguments are parsed.
/// </summary>
///
/// <author>Almighty-Shogun</author>
/// <since>Unreleased</since>
public enum ArgumentParsingMode
{
    /// <summary>
    /// Splits arguments on whitespace.
    /// </summary>
    Spaces,
    
    /// <summary>
    /// Supports quoted arguments containing whitespace.
    /// </summary>
    Quotes
}
