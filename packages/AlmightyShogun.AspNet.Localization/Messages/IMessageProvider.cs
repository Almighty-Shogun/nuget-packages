namespace AlmightyShogun.AspNet.Localization;

/// <summary>
/// Provides localized messages for a language.
/// </summary>
///
/// <author>Almighty-Shogun</author>
/// <since>4.0.0</since>
internal interface IMessageProvider
{
    /// <summary>
    /// Gets the messages defined for a language.
    /// </summary>
    ///
    /// <param name="language">The language to retrieve messages for.</param>
    ///
    /// <returns>
    /// The messages keyed by their dot-separated keys, or an empty dictionary if none are available.
    /// </returns>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>4.0.0</since>
    IReadOnlyDictionary<string, string> GetMessages(string language);
}
