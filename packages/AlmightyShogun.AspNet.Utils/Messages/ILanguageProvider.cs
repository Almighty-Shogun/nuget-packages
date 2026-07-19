namespace AlmightyShogun.AspNet.Utils;

/// <summary>
/// Provides the active language used for HTTP message resolution.
/// </summary>
///
/// <author>Almighty-Shogun</author>
/// <since>Unreleased</since>
internal interface ILanguageProvider
{
    /// <summary>
    /// Gets the language code used to resolve HTTP messages.
    /// </summary>
    ///
    /// <returns>The language code to use for message resolution.</returns>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    string GetLanguage();

    /// <summary>
    /// Sets the response content language for the resolved message.
    /// </summary>
    ///
    /// <param name="language">The language code used by the response content.</param>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    void SetContentLanguage(string language);
}
