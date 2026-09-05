using System.Collections.Frozen;

namespace AlmightyShogun.Serilog;

/// <summary>
/// Holds the ANSI foreground codes the formatter writes, and the shorthand table a message template uses to pick one.
/// Every code here selects a foreground color, apart from <see cref="Reset"/>, which is SGR 0 and clears every attribute.
/// </summary>
///
/// <author>Almighty-Shogun</author>
/// <since>1.0.0</since>
internal static class AnsiColor
{
    /// <summary>
    /// Returns the terminal to its default colors. Written after every colored span, so a log line never leaks its color
    /// into whatever the terminal prints next.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>1.0.0</since>
    internal const string Reset = "\e[0m";

    /// <summary>
    /// Red foreground, selected by the <c>r</c> shorthand and used for the <c>Error</c> level.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>1.0.0</since>
    internal const string Red = "\e[31m";

    /// <summary>
    /// Blue foreground, reachable only through the <c>b</c> shorthand. No level or value type uses it by default.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>1.0.0</since>
    private const string Blue = "\e[34m";

    /// <summary>
    /// Cyan foreground, selected by the <c>c</c> shorthand and used for numeric property values.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>1.0.0</since>
    internal const string Cyan = "\e[36m";

    /// <summary>
    /// Green foreground, selected by the <c>g</c> shorthand and used for the <c>Information</c> level.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>1.0.0</since>
    internal const string Green = "\e[32m";

    /// <summary>
    /// Yellow foreground, selected by the <c>y</c> shorthand and used for the <c>Warning</c> level.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>1.0.0</since>
    internal const string Yellow = "\e[33m";

    /// <summary>
    /// Magenta foreground, selected by the <c>m</c> shorthand and used for boolean property values.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>1.0.0</since>
    internal const string Magenta = "\e[35m";

    /// <summary>
    /// White foreground, the fallback whenever nothing more specific applies: the <c>Verbose</c> and <c>Debug</c>
    /// levels, string values, and any shorthand that is not recognized.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>1.0.0</since>
    internal const string White = "\e[37m";

    /// <summary>
    /// Dark gray foreground, used for null property values and for the exception block appended below a line.
    /// No shorthand maps to it, so a template author cannot select it by name.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>1.0.0</since>
    internal const string DarkGray = "\e[90m";

    /// <summary>
    /// Bright red foreground, selected by the <c>br</c> shorthand and used for the <c>Fatal</c> level.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>1.0.0</since>
    internal const string BrightRed = "\e[91m";

    /// <summary>
    /// Bright blue foreground, reachable only through the <c>bb</c> shorthand.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>1.0.0</since>
    private const string BrightBlue = "\e[94m";

    /// <summary>
    /// Bright cyan foreground, reachable only through the <c>bc</c> shorthand.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>1.0.0</since>
    private const string BrightCyan = "\e[96m";

    /// <summary>
    /// Bright green foreground, reachable only through the <c>bg</c> shorthand.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>1.0.0</since>
    private const string BrightGreen = "\e[92m";

    /// <summary>
    /// Bright yellow foreground, reachable only through the <c>by</c> shorthand.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>1.0.0</since>
    private const string BrightYellow = "\e[93m";

    /// <summary>
    /// Bright magenta foreground, reachable only through the <c>bm</c> shorthand.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>1.0.0</since>
    private const string BrightMagenta = "\e[95m";

    /// <summary>
    /// Maps every supported shorthand color code to its ANSI escape code. Adding a shorthand here is all that is needed to
    /// support it, since a template's color spec is taken as whatever follows the pipe and looked up directly.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    private static readonly FrozenDictionary<string, string> ShortCodes = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
    {
        ["r"] = Red,
        ["g"] = Green,
        ["b"] = Blue,
        ["c"] = Cyan,
        ["y"] = Yellow,
        ["m"] = Magenta,
        ["br"] = BrightRed,
        ["bg"] = BrightGreen,
        ["bb"] = BrightBlue,
        ["bc"] = BrightCyan,
        ["by"] = BrightYellow,
        ["bm"] = BrightMagenta
    }.ToFrozenDictionary(StringComparer.OrdinalIgnoreCase);

    /// <summary>
    /// Resolves a shorthand written in a message template into the escape code to emit.
    /// </summary>
    ///
    /// <param name="shortCode">
    /// The shorthand taken from the part of a format specifier after <c>|</c>. Matched without regard to case, so
    /// <c>BR</c> and <c>br</c> both reach bright red.
    /// </param>
    ///
    /// <returns>
    /// The matching escape code, or <see cref="White"/> for anything unrecognized. An unknown shorthand therefore prints in
    /// the fallback color rather than failing the log write, which keeps a typo in a template from losing the line.
    /// </returns>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>1.0.0</since>
    internal static string FromShort(string shortCode) => ShortCodes.GetValueOrDefault(shortCode, White);
}
