using System.Text.RegularExpressions;

namespace AlmightyShogun.AspNet.Localization;

/// <summary>
/// Provides validation for language tags.
/// </summary>
///
/// <author>Almighty-Shogun</author>
/// <since>4.0.0</since>
internal static partial class LanguageTag
{
    /// <summary>
    /// The pattern used to validate language tags.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>4.0.0</since>
    internal const string Pattern = @"^[A-Za-z]{2,3}(-[A-Za-z0-9]{2,8})*\z";

    /// <summary>
    /// Determines whether the specified language tag is valid.
    /// </summary>
    ///
    /// <param name="language">The language tag to validate.</param>
    ///
    /// <returns><c>true</c> if <paramref name="language"/> is valid; otherwise, <c>false</c>.</returns>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>4.0.0</since>
    internal static bool IsValid(string language) => LanguageTagRegex().IsMatch(language);
    
    [GeneratedRegex(Pattern)]
    private static partial Regex LanguageTagRegex();
}
