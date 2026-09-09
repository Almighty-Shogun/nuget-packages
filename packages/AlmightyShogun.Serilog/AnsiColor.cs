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
    /// SGR 0, which returns the terminal to its default colors.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>1.0.0</since>
    internal const string Reset = "\e[0m";

    /// <summary>
    /// Red foreground, SGR 31, selected by the <c>r</c> shorthand.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>1.0.0</since>
    internal const string Red = "\e[31m";

    /// <summary>
    /// Blue foreground, SGR 34, reachable only through the <c>b</c> shorthand.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>1.0.0</since>
    private const string _blue = "\e[34m";

    /// <summary>
    /// Cyan foreground, SGR 36, selected by the <c>c</c> shorthand.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>1.0.0</since>
    internal const string Cyan = "\e[36m";

    /// <summary>
    /// Green foreground, SGR 32, selected by the <c>g</c> shorthand.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>1.0.0</since>
    internal const string Green = "\e[32m";

    /// <summary>
    /// Yellow foreground, SGR 33, selected by the <c>y</c> shorthand.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>1.0.0</since>
    internal const string Yellow = "\e[33m";

    /// <summary>
    /// Magenta foreground, SGR 35, selected by the <c>m</c> shorthand.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>1.0.0</since>
    internal const string Magenta = "\e[35m";

    /// <summary>
    /// White foreground, SGR 37. No shorthand maps to it, but <see cref="FromShort"/> returns it for every code the table
    /// does not hold, so any unrecognized shorthand selects it.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>1.0.0</since>
    internal const string White = "\e[37m";

    /// <summary>
    /// Dark gray foreground, SGR 90. No shorthand maps to it, so a template author cannot select it by name.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>1.0.0</since>
    internal const string DarkGray = "\e[90m";

    /// <summary>
    /// Bright red foreground, SGR 91, selected by the <c>br</c> shorthand.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>1.0.0</since>
    internal const string BrightRed = "\e[91m";

    /// <summary>
    /// Bright blue foreground, SGR 94, reachable only through the <c>bb</c> shorthand.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>1.0.0</since>
    private const string _brightBlue = "\e[94m";

    /// <summary>
    /// Bright cyan foreground, SGR 96, reachable only through the <c>bc</c> shorthand.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>1.0.0</since>
    private const string _brightCyan = "\e[96m";

    /// <summary>
    /// Bright green foreground, SGR 92, reachable only through the <c>bg</c> shorthand.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>1.0.0</since>
    private const string _brightGreen = "\e[92m";

    /// <summary>
    /// Bright yellow foreground, SGR 93, reachable only through the <c>by</c> shorthand.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>1.0.0</since>
    private const string _brightYellow = "\e[93m";

    /// <summary>
    /// Bright magenta foreground, SGR 95, reachable only through the <c>bm</c> shorthand.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>1.0.0</since>
    private const string _brightMagenta = "\e[95m";

    /// <summary>
    /// Maps every supported shorthand color code to its ANSI escape code. Adding a shorthand here is all that is needed to
    /// support it, since a template's color spec is taken as whatever follows the pipe and looked up directly.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>4.0.0</since>
    private static readonly FrozenDictionary<string, string> _shortCodes = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
    {
        ["r"] = Red,
        ["g"] = Green,
        ["b"] = _blue,
        ["c"] = Cyan,
        ["y"] = Yellow,
        ["m"] = Magenta,
        ["br"] = BrightRed,
        ["bg"] = _brightGreen,
        ["bb"] = _brightBlue,
        ["bc"] = _brightCyan,
        ["by"] = _brightYellow,
        ["bm"] = _brightMagenta
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
    internal static string FromShort(string shortCode) => _shortCodes.GetValueOrDefault(shortCode, White);
}
