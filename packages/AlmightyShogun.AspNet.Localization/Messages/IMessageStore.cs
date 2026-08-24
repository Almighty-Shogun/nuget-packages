namespace AlmightyShogun.AspNet.Localization;

/// <summary>
/// Supplies the messages defined for one language, already flattened to dot-separated keys. Knows nothing about fallback
/// or negotiation: choosing which languages to ask for belongs to <see cref="IMessageResolver"/>.
/// </summary>
///
/// <author>Almighty-Shogun</author>
/// <since>Unreleased</since>
internal interface IMessageStore
{
    /// <summary>
    /// Gets the flattened messages for one language, without trying any other.
    /// </summary>
    ///
    /// <param name="language">
    /// The exact language tag to look up. Treated as a literal directory name, so <c>nl-BE</c> and <c>nl</c> are
    /// separate lookups and neither stands in for the other, and so a tag carrying anything but letters, digits, and
    /// hyphens resolves nothing at all.
    /// </param>
    ///
    /// <returns>
    /// The messages keyed by their dot-separated key, or an empty dictionary when the language has no message files or
    /// its tag was rejected. An empty result is how the caller tells an undefined language from a defined but silent one.
    /// </returns>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    IReadOnlyDictionary<string, string> GetMessages(string language);
}
