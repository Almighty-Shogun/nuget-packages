using System.Collections.Frozen;

namespace AlmightyShogun.Serilog;

/// <summary>
/// Provides ANSI color codes and shorthand color resolution for code formatting.
/// </summary>
///
/// <author>Almighty-Shogun</author>
/// <since>1.0.0</since>
internal static class AnsiColor
{
    internal const string Reset = "\e[0m";
    internal const string Red = "\e[31m";
    private const string _blue = "\e[34m";
    internal const string Cyan = "\e[36m";
    internal const string Green = "\e[32m";
    internal const string Yellow = "\e[33m";
    internal const string Magenta = "\e[35m";
    internal const string White = "\e[37m";
    internal const string DarkGray = "\e[90m";
    internal const string BrightRed = "\e[91m";
    private const string _brightBlue = "\e[94m";
    private const string _brightCyan = "\e[96m";
    private const string _brightGreen = "\e[92m";
    private const string _brightYellow = "\e[93m";
    private const string _brightMagenta = "\e[95m";

    /// <summary>
    /// Maps supported color shorthands to their ANSI escape codes.
    /// </summary>
    ///
    /// <remarks>Color shorthands are marked case-insensitively.</remarks>
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
    /// Resolves a color shorthand to its ANSI escape code.
    /// </summary>
    ///
    /// <param name="shortCode">
    /// The shorthand to resolve. Matching is case-insensitive.
    /// </param>
    ///
    /// <returns>
    /// The matching ANSI escape code, or <see cref="White"/> if the shorthand is unrecognized.
    /// </returns>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>1.0.0</since>
    internal static string FromShort(string shortCode) => _shortCodes.GetValueOrDefault(shortCode, White);
}
