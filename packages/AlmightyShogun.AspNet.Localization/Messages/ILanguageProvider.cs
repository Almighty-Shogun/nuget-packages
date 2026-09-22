namespace AlmightyShogun.AspNet.Localization;

/// <summary>
/// Provides language preferences for message resolution.
/// </summary>
///
/// <author>Almighty-Shogun</author>
/// <since>4.0.0</since>
public interface ILanguageProvider
{
    /// <summary>
    /// Gets the preferred language.
    /// </summary>
    ///
    /// <returns>
    /// The preferred language.
    /// </returns>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>4.0.0</since>
    string GetLanguage();

    /// <summary>
    /// Gets the preferred languages in preference order.
    /// </summary>
    ///
    /// <returns>The preferred languages in preference order.</returns>
    ///
    /// <remarks>
    /// By default, returns only <see cref="GetLanguage"/>.
    /// </remarks>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>4.0.0</since>
    IReadOnlyList<string> GetLanguages() => [GetLanguage()];
}
