namespace AlmightyShogun.Logging;

internal static class AnsiColor
{
    public const string Reset = "\e[0m";

    internal const string Red = "\e[31m";
    private const string Blue = "\e[34m";
    internal const string Cyan = "\e[36m";
    internal const string Green = "\e[32m";
    internal const string Yellow = "\e[33m";
    internal const string Magenta = "\e[35m";
    internal const string White = "\e[37m";

    internal const string DarkGray = "\e[90m";
    internal const string BrightRed = "\e[91m";
    private const string BrightBlue = "\e[94m";
    private const string BrightCyan = "\e[96m";
    private const string BrightGreen = "\e[92m";
    private const string BrightYellow = "\e[93m";
    private const string BrightMagenta = "\e[95m";

    /// <summary>
    /// Converts a shorthand color code into its corresponding ANSI escape code.
    /// </summary>
    /// 
    /// <param name="shortCode">The shorthand representation of the color (e.g., "r" for red, "g" for green, etc.)</param>
    /// 
    /// <returns>The ANSI escape code corresponding to the color. If the shorthand code is not recognized, a default white color is returned.</returns>
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
