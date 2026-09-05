using System.Globalization;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace AlmightyShogun.AspNet.Localization;

/// <summary>
/// Resolves HTTP messages by negotiating one language for the request, the first candidate in the fallback chain whose
/// directory holds any messages, then looking every key up in that language alone. An unresolvable key comes back as
/// itself, so a missing translation shows up in the response instead of taking the request down. A key a message file
/// defines as an empty string still resolves to one. Reading the message files can still fail, and nothing here catches
/// what <see cref="IMessageProvider.GetMessages"/> throws.
/// </summary>
///
/// <param name="messageProvider">
/// The provider the candidates are looked up in. Negotiation queries it once per candidate and stops at the first that
/// answers anything, and the key is then read from that same language rather than from the rest of the chain.
/// </param>
/// <param name="languageProvider">The provider supplying the accepted languages the fallback chain is built from.</param>
/// <param name="localizationOptions">The settings supplying the default language that ends every fallback chain.</param>
/// <param name="logger">
/// The logger an unresolved key and a template the values do not fit are reported on, both at warning level. Resolution
/// continues either way, returning the key itself or the unformatted template.
/// </param>
///
/// <author>Almighty-Shogun</author>
/// <since>Unreleased</since>
internal sealed class JsonMessageResolver(
    IMessageProvider messageProvider,
    ILanguageProvider languageProvider,
    IOptions<LocalizationSettings> localizationOptions,
    ILogger<JsonMessageResolver> logger
) : IMessageResolver
{
    /// <inheritdoc />
    ///
    /// <exception cref="DirectoryNotFoundException">
    /// A language directory was removed between the provider finding it and enumerating its files.
    /// </exception>
    /// <exception cref="UnauthorizedAccessException">
    /// The process may not list the files of a language directory it can see.
    /// </exception>
    /// <exception cref="NullReferenceException">
    /// A custom <see cref="ILanguageProvider"/> returned <c>null</c> from <see cref="ILanguageProvider.GetLanguages"/>.
    /// </exception>
    public string Resolve(string key) => Resolve(key, []);

    /// <inheritdoc />
    ///
    /// <exception cref="DirectoryNotFoundException">
    /// A language directory was removed between the provider finding it and enumerating its files.
    /// </exception>
    /// <exception cref="UnauthorizedAccessException">
    /// The process may not list the files of a language directory it can see.
    /// </exception>
    /// <exception cref="NullReferenceException">
    /// A custom <see cref="ILanguageProvider"/> returned <c>null</c> from <see cref="ILanguageProvider.GetLanguages"/>.
    /// </exception>
    ///
    /// <remarks>
    /// Only <see cref="FormatException"/> is caught while substituting, so an exception raised by a value's own
    /// <see cref="object.ToString"/> reaches the caller.
    /// </remarks>
    public string Resolve(string key, IReadOnlyList<object?> parameters)
    {
        string language = ResolveLanguage();

        IReadOnlyDictionary<string, string> messages = messageProvider.GetMessages(language);

        if (messages.TryGetValue(key, out string? template))
            return Format(template, parameters, language);

        logger.LogWarning("No message found for key {MessageKey} in {Language}", key, language);

        return key;
    }

    /// <inheritdoc />
    ///
    /// <exception cref="DirectoryNotFoundException">
    /// A language directory was removed between the provider finding it and enumerating its files.
    /// </exception>
    /// <exception cref="UnauthorizedAccessException">
    /// The process may not list the files of a language directory it can see.
    /// </exception>
    /// <exception cref="NullReferenceException">
    /// A custom <see cref="ILanguageProvider"/> returned <c>null</c> from <see cref="ILanguageProvider.GetLanguages"/>.
    /// </exception>
    public string ResolveLanguage()
    {
        foreach (string language in GetLanguageCandidates())
            if (messageProvider.GetMessages(language).Count > 0)
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
    /// <exception cref="NullReferenceException">
    /// A custom <see cref="ILanguageProvider"/> returned <c>null</c>, which is enumerated without being checked.
    /// </exception>
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
    /// written in the server's culture while the surrounding words are in the caller's.
    /// </param>
    ///
    /// <returns>
    /// The formatted message, or the unformatted template when the placeholders and the values do not agree. A visible
    /// placeholder in a response is preferred over an exception thrown while building an error body.
    /// </returns>
    ///
    /// <remarks>
    /// The catch is narrowed to <see cref="FormatException"/>, so an exception raised by a value's own
    /// <see cref="object.ToString"/> is not absorbed here and reaches the caller.
    /// </remarks>
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
