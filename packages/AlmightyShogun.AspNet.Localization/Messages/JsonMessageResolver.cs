using System.Globalization;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace AlmightyShogun.AspNet.Localization;

/// <summary>
/// Resolves HTTP messages by negotiating one language for the request, the first candidate in the fallback chain whose
/// directory holds any messages, then looking every key up in that language alone. Never throws and never returns
/// blank: an unresolvable key comes back as itself, so a missing translation shows up in the response instead of taking
/// the request down.
/// </summary>
///
/// <param name="messageStore">
/// The store the candidates are looked up in. Negotiation queries it once per candidate and stops at the first that
/// answers anything, and the key is then read from that same language rather than from the rest of the chain.
/// </param>
/// <param name="languageProvider">The provider supplying the accepted languages the fallback chain is built from.</param>
/// <param name="localizationOptions">The settings supplying the default language that ends every fallback chain.</param>
/// <param name="logger">
/// The logger an unresolved key is reported on, at warning level. Resolution still succeeds by returning the key, so
/// this log line is the only trace a translation is missing.
/// </param>
///
/// <author>Almighty-Shogun</author>
/// <since>Unreleased</since>
internal sealed class JsonMessageResolver(
    IMessageStore messageStore,
    ILanguageProvider languageProvider,
    IOptions<LocalizationSettings> localizationOptions,
    ILogger<JsonMessageResolver> logger
) : IMessageResolver
{
    /// <inheritdoc />
    public string Resolve(string key) => Resolve(key, []);

    /// <inheritdoc />
    public string Resolve(string key, IReadOnlyList<object?> parameters)
    {
        string language = ResolveLanguage();

        IReadOnlyDictionary<string, string> messages = messageStore.GetMessages(language);

        if (messages.TryGetValue(key, out string? template))
            return Format(template, parameters, language);

        logger.LogWarning("No message found for key {MessageKey} in {Language}", key, language);

        return key;
    }

    /// <inheritdoc />
    public string ResolveLanguage()
    {
        foreach (string language in GetLanguageCandidates())
            if (messageStore.GetMessages(language).Count > 0)
                return language;

        return localizationOptions.Value.DefaultLanguage;
    }

    /// <summary>
    /// Builds the language fallback chain for the current request.
    /// </summary>
    ///
    /// <returns>
    /// Each accepted language in preference order, each followed by its own progressively shorter forms, with the
    /// configured default language last. Every candidate appears once.
    /// </returns>
    ///
    /// <remarks>
    /// A tag's shorter forms follow it immediately rather than after every other accepted language, because <c>nl</c> is
    /// a closer match for a client asking for <c>nl-BE</c> than a lower-ranked <c>fr</c>.
    /// </remarks>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    private IEnumerable<string> GetLanguageCandidates()
    {
        HashSet<string> seen = new(StringComparer.OrdinalIgnoreCase);

        foreach (string language in languageProvider.GetLanguages())
        {
            if (seen.Add(language))
                yield return language;

            foreach (string fallbackLanguage in GetFallbackTags(language))
                if (seen.Add(fallbackLanguage))
                    yield return fallbackLanguage;
        }

        string defaultLanguage = localizationOptions.Value.DefaultLanguage;

        if (seen.Add(defaultLanguage))
            yield return defaultLanguage;
    }

    /// <summary>
    /// Walks a language tag back one subtag at a time, so every level a deployment might define a directory for is
    /// offered before the next accepted language is.
    /// </summary>
    ///
    /// <param name="language">The tag to strip, such as <c>zh-Hant-TW</c>.</param>
    ///
    /// <returns>
    /// The tag without its last subtag, then without the one before it, and so on down to the primary subtag. Empty for
    /// a tag that is already primary or that begins with a hyphen and so has no primary subtag to keep.
    /// </returns>
    ///
    /// <remarks>
    /// Dropping straight to the primary subtag would skip the script, which for <c>zh-Hant-TW</c> is the level that
    /// decides whether the reader gets Traditional or Simplified text. Stripping one at a time offers <c>zh-Hant</c>
    /// before <c>zh</c>, so a deployment that separates the two is matched instead of falling past both.
    /// </remarks>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    private static IEnumerable<string> GetFallbackTags(string language)
    {
        for (int index = language.LastIndexOf('-'); index > 0; index = language.LastIndexOf('-', index - 1))
            yield return language[..index];
    }

    /// <summary>
    /// Substitutes parameters into a resolved template, treating a mismatch as a formatting problem rather than a fault.
    /// </summary>
    ///
    /// <param name="template">The message text, holding <c>{0}</c>-style placeholders for each expected value.</param>
    /// <param name="parameters">
    /// The values to substitute by position. An empty list short-circuits, so a template holding literal braces is
    /// returned untouched when no parameters were passed.
    /// </param>
    /// <param name="language">
    /// The language the template was resolved in, used to format the values. Without it a number or a date would be
    /// written in the server's culture while the words around it are in the caller's.
    /// </param>
    ///
    /// <returns>
    /// The formatted message, or the unformatted template when the placeholders and the values do not agree. A visible
    /// placeholder in a response is preferred over an exception thrown while building an error body.
    /// </returns>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    private string Format(string template, IReadOnlyList<object?> parameters, string language)
    {
        if (parameters.Count == 0)
            return template;

        try
        {
            return string.Format(GetCulture(language), template, parameters.ToArray());
        }
        catch (FormatException)
        {
            logger.LogWarning("Failed to format template \"{Template}\"", template);

            return template;
        }
    }

    /// <summary>
    /// Resolves the culture a message's values are formatted with, falling back rather than failing when the tag names
    /// no culture the runtime knows.
    /// </summary>
    ///
    /// <param name="language">The negotiated language tag, already known to be well-formed but not to be a real culture.</param>
    ///
    /// <returns>
    /// The matching culture, or <see cref="CultureInfo.InvariantCulture"/> when none exists. Invariant is the safer
    /// miss: it formats predictably instead of borrowing whichever culture the server happens to run under.
    /// </returns>
    ///
    /// <remarks>
    /// <see cref="CultureInfo.GetCultureInfo(string)"/> caches, so this costs a dictionary lookup per formatted message
    /// rather than building a culture each time. A message directory named for something that is not a culture, which
    /// the tag check allows, therefore degrades to invariant formatting instead of throwing mid-response.
    /// </remarks>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    private static CultureInfo GetCulture(string language)
    {
        try
        {
            return CultureInfo.GetCultureInfo(language);
        }
        catch (CultureNotFoundException)
        {
            return CultureInfo.InvariantCulture;
        }
    }
}
