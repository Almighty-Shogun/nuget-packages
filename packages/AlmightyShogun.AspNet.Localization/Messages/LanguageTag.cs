using System.Text.RegularExpressions;

namespace AlmightyShogun.AspNet.Localization;

/// <summary>
/// Decides whether a string may be used as a language tag. Every value that reaches the filesystem as a message
/// directory name passes through here, so the check lives beside the store rather than at the boundary that happens to
/// produce the value.
/// </summary>
///
/// <author>Almighty-Shogun</author>
/// <since>Unreleased</since>
internal static partial class LanguageTag
{
    /// <summary>
    /// The shape a tag has to match: two or three letters, then any number of <c>-</c> separated parts of two to eight
    /// letters or digits. Exposed so the configured default is validated against the same rule at startup, rather than
    /// failing quietly at resolve time.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    internal const string Pattern = "^[A-Za-z]{2,3}(-[A-Za-z0-9]{2,8})*$";

    /// <summary>
    /// Determines whether a tag is well-formed enough to be trusted as a directory name.
    /// </summary>
    ///
    /// <param name="language">The tag to check, from a request header, from configuration, or from a custom provider.</param>
    ///
    /// <returns>
    /// <c>true</c> for a two or three letter primary subtag followed by any number of two to eight character
    /// alphanumeric subtags; otherwise <c>false</c>, which includes an empty value, a longer primary subtag such as
    /// <c>english</c>, and anything carrying a path separator, a drive letter, or a relative segment.
    /// </returns>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    internal static bool IsValid(string language) => LanguageTagRegex().IsMatch(language);

    /// <summary>
    /// Matches a well-formed language tag. Anything else is rejected, because the value reaches the filesystem when
    /// message files are resolved and an unvalidated tag would allow directory traversal.
    /// </summary>
    ///
    /// <returns>
    /// The generated matcher for a two or three letter primary tag followed by any number of alphanumeric subtags. It
    /// is deliberately narrower than the grammar in the specification, since anything broader admits a path separator.
    /// </returns>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    [GeneratedRegex(Pattern)]
    private static partial Regex LanguageTagRegex();
}
