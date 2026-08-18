using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace AlmightyShogun.AspNet.Utils;

/// <summary>
/// Resolves HTTP messages by walking the language fallback chain and looking each candidate up in the message store,
/// stopping at the first language that defines the key. Never throws and never returns blank: an unresolvable key comes
/// back as itself, so a missing translation shows up in the response instead of taking the request down.
/// </summary>
///
/// <param name="messageStore">
/// The store each candidate language is looked up in. It is queried once per candidate, so a chain is only as long as
/// the first language that answers.
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
        foreach (string language in GetLanguageCandidates())
        {
            IReadOnlyDictionary<string, string> messages = messageStore.GetMessages(language);

            if (messages.TryGetValue(key, out string? template))
                return Format(template, parameters);
        }

        logger.LogWarning("No message found for key {MessageKey}", key);

        return key;
    }

    /// <inheritdoc />
    public string ResolveLanguage()
    {
        foreach (string language in GetLanguageCandidates())
        {
            if (messageStore.GetMessages(language).Count > 0)
                return language;
        }

        return localizationOptions.Value.DefaultLanguage;
    }

    /// <summary>
    /// Builds the language fallback chain for the current request.
    /// </summary>
    ///
    /// <returns>
    /// Each accepted language in preference order, each followed by its neutral form, with the configured default
    /// language last. Every candidate appears once.
    /// </returns>
    ///
    /// <remarks>
    /// A culture-specific tag is followed immediately by its neutral form rather than after every other accepted
    /// language, because <c>nl</c> is a closer match for a client asking for <c>nl-BE</c> than a lower-ranked <c>fr</c>.
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

            if (GetNeutralLanguage(language) is { } neutralLanguage && seen.Add(neutralLanguage))
                yield return neutralLanguage;
        }

        string defaultLanguage = localizationOptions.Value.DefaultLanguage;

        if (seen.Add(defaultLanguage))
            yield return defaultLanguage;
    }

    /// <summary>
    /// Resolves the neutral language for a culture-specific language tag.
    /// </summary>
    ///
    /// <param name="language">The tag to strip, such as <c>nl-BE</c>.</param>
    ///
    /// <returns>
    /// The part before the first hyphen, or <c>null</c> when the tag is already neutral or begins with a hyphen and so
    /// has no primary subtag to keep.
    /// </returns>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    private static string? GetNeutralLanguage(string language)
    {
        int separatorIndex = language.IndexOf('-', StringComparison.Ordinal);

        return separatorIndex <= 0 ? null : language[..separatorIndex];
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
    ///
    /// <returns>
    /// The formatted message, or the unformatted template when the placeholders and the values do not agree. A visible
    /// placeholder in a response is preferred over an exception thrown while building an error body.
    /// </returns>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    private static string Format(string template, IReadOnlyList<object?> parameters)
    {
        if (parameters.Count == 0)
            return template;

        try
        {
            return string.Format(template, parameters.ToArray());
        }
        catch (FormatException)
        {
            return template;
        }
    }
}
