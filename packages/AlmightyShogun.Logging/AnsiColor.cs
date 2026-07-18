namespace AlmightyShogun.Logging;

/// <summary>
/// Provides ANSI escape codes used by the color formatter.
/// </summary>
///
/// <author>Almighty-Shogun</author>
/// <since>1.0.0</since>
internal static class AnsiColor
{
    /// <summary>
    /// Resets the console color back to the terminal default.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>1.0.0</since>
    public const string Reset = "\e[0m";

    /// <summary>
    /// Gets the ANSI code for red foreground text.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>1.0.0</since>
    internal const string Red = "\e[31m";

    /// <summary>
    /// Gets the ANSI code for blue foreground text.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>1.0.0</since>
    private const string Blue = "\e[34m";

    /// <summary>
    /// Gets the ANSI code for cyan foreground text.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>1.0.0</since>
    internal const string Cyan = "\e[36m";

    /// <summary>
    /// Gets the ANSI code for green foreground text.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>1.0.0</since>
    internal const string Green = "\e[32m";

    /// <summary>
    /// Gets the ANSI code for yellow foreground text.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>1.0.0</since>
    internal const string Yellow = "\e[33m";

    /// <summary>
    /// Gets the ANSI code for magenta foreground text.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>1.0.0</since>
    internal const string Magenta = "\e[35m";

    /// <summary>
    /// Gets the ANSI code for white foreground text.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>1.0.0</since>
    internal const string White = "\e[37m";

    /// <summary>
    /// Gets the ANSI code for dark gray foreground text.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>1.0.0</since>
    internal const string DarkGray = "\e[90m";

    /// <summary>
    /// Gets the ANSI code for bright red foreground text.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>1.0.0</since>
    internal const string BrightRed = "\e[91m";

    /// <summary>
    /// Gets the ANSI code for bright blue foreground text.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>1.0.0</since>
    private const string BrightBlue = "\e[94m";

    /// <summary>
    /// Gets the ANSI code for bright cyan foreground text.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>1.0.0</since>
    private const string BrightCyan = "\e[96m";

    /// <summary>
    /// Gets the ANSI code for bright green foreground text.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>1.0.0</since>
    private const string BrightGreen = "\e[92m";

    /// <summary>
    /// Gets the ANSI code for bright yellow foreground text.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>1.0.0</since>
    private const string BrightYellow = "\e[93m";

    /// <summary>
    /// Gets the ANSI code for bright magenta foreground text.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>1.0.0</since>
    private const string BrightMagenta = "\e[95m";

    /// <summary>
    /// Converts a shorthand color code into its corresponding ANSI escape code.
    /// </summary>
    ///
    /// <param name="shortCode">The shorthand color code, such as <c>r</c> for red or <c>bg</c> for bright green.</param>
    ///
    /// <returns>The matching ANSI escape code, or white when the shorthand code is not recognized.</returns>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>1.0.0</since>
    public static string FromShort(string shortCode) => shortCode.ToLowerInvariant() switch
    {
        "r" => Red,
        "g" => Green,
        "b" => Blue,
        "c" => Cyan,
        "y" => Yellow,
        "m" => Magenta,
        "br" => BrightRed,
        "bg" => BrightGreen,
        "bb" => BrightBlue,
        "bc" => BrightCyan,
        "by" => BrightYellow,
        "bm" => BrightMagenta,
        _ => White
    };
}
