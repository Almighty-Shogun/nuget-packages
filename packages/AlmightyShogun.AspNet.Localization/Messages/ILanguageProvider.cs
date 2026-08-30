namespace AlmightyShogun.AspNet.Localization;

/// <summary>
/// Provides the language used for HTTP message resolution. The default implementation reads the request
/// <c>Accept-Language</c> header; register a replacement to negotiate from a cookie, a route value, or a user profile.
/// A returned tag becomes a directory name, so one carrying anything but letters, digits, and hyphens resolves nothing.
/// </summary>
///
/// <author>Almighty-Shogun</author>
/// <since>Unreleased</since>
public interface ILanguageProvider
{
    /// <summary>
    /// Gets the single language to resolve messages in, ignoring any lower-ranked alternative the caller would accept.
    /// </summary>
    ///
    /// <returns>
    /// A language tag such as <c>nl-BE</c> or <c>en</c>. Never blank: an implementation with nothing to negotiate from
    /// returns the configured default rather than an empty value, since the result is looked up as-is.
    /// </returns>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    string GetLanguage();

    /// <summary>
    /// Gets every language the caller accepts, in preference order, so message resolution can try a lower-ranked
    /// language before falling back to the configured default.
    /// </summary>
    ///
    /// <returns>The accepted languages in preference order. Never empty.</returns>
    ///
    /// <remarks>
    /// This interface's own default body returns only <see cref="GetLanguage"/>, so a provider written before this
    /// member existed keeps working unchanged. The package's header-reading provider overrides it, and so should any
    /// provider whose source of the language is itself ranked.
    /// </remarks>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    IReadOnlyList<string> GetLanguages() => [GetLanguage()];
}
