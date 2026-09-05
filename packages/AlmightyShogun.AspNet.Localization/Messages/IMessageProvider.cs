namespace AlmightyShogun.AspNet.Localization;

/// <summary>
/// Supplies the messages defined for one language, already flattened to dot-separated keys. Knows nothing about fallback
/// or negotiation: choosing which languages to ask for belongs to <see cref="IMessageResolver"/>.
/// </summary>
///
/// <author>Almighty-Shogun</author>
/// <since>Unreleased</since>
internal interface IMessageProvider
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
    /// The messages keyed by their dot-separated key, or an empty dictionary. A rejected tag, a language no directory
    /// holds messages for, and a directory whose files define no messages all come back the same way, so the caller
    /// cannot tell them apart.
    /// </returns>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    IReadOnlyDictionary<string, string> GetMessages(string language);
}
