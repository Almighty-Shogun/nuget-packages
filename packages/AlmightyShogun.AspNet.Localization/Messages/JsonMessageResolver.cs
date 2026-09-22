using System.Globalization;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace AlmightyShogun.AspNet.Localization;

/// <summary>
/// Resolves localized messages using the preferred available language.
/// </summary>
///
/// <param name="messageProvider">The message provider.</param>
/// <param name="languageProvider">The language preference provider.</param>
/// <param name="localizationOptions">The settings supplying the default language that ends every fallback chain.</param>
/// <param name="logger"> The localization settings.</param>
///
/// <author>Almighty-Shogun</author>
/// <since>4.0.0</since>
internal sealed class JsonMessageResolver(
    IMessageProvider messageProvider,
    ILanguageProvider languageProvider,
    IOptions<LocalizationSettings> localizationOptions,
    ILogger<JsonMessageResolver> logger
) : IMessageResolver
{
    /// <inheritdoc />
    public string Resolve(string key) => Resolve(key, []);

    /// <inheritdoc />
    ///
    /// <remarks>
    /// Message parameters are formatted using the culture of the resolved language.
    /// If formatting fails, the unformatted template is returned.
    /// </remarks>
    public string Resolve(string key, IReadOnlyList<object?> parameters)
    {
        (string language, IReadOnlyDictionary<string, string> messages) = ResolveMessages();

        if (messages.TryGetValue(key, out string? template))
            return Format(template, parameters, language);

        logger.LogWarning("No message found for key {MessageKey} in {Language}", key, language);

        return key;
    }

    /// <inheritdoc />
    public string ResolveLanguage()
    {
        (string language, _) = ResolveMessages();

        return language;
    }

    /// <summary>
    /// Resolves the preferred available language and its messages.
    /// </summary>
    ///
    /// <returns>The resolved language and its message snapshot.</returns>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    private (string Language, IReadOnlyDictionary<string, string> Messages) ResolveMessages()
    {
        string defaultLanguage = localizationOptions.Value.DefaultLanguage;
        IReadOnlyDictionary<string, string>? defaultMessages = null;

        foreach (string language in GetLanguageCandidates())
        {
            IReadOnlyDictionary<string, string> messages = messageProvider.GetMessages(language);

            if (language.Equals(defaultLanguage, StringComparison.OrdinalIgnoreCase))
                defaultMessages = messages;

            if (messages.Count > 0)
                return (language, messages);
        }

        return (defaultLanguage, defaultMessages ?? messageProvider.GetMessages(defaultLanguage));
    }

    /// <summary>
    /// Enumerates preferred languages and their fallbacks, followed by the configured default language.
    /// </summary>
    ///
    /// <returns>The language candidates in resolution order.</returns>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>4.0.0</since>
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
    /// Enumerates progressively less specific forms of a language tag.
    /// </summary>
    ///
    /// <param name="language">The language tag.</param>
    ///
    /// <returns>The fallback language tags.</returns>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>4.0.0</since>
    private static IEnumerable<string> GetFallbackTags(string language)
    {
        for (int index = language.LastIndexOf('-'); index > 0; index = language.LastIndexOf('-', index - 1))
            yield return language[..index];
    }

    /// <summary>
    /// Formats a message template using the resolved language.
    /// </summary>
    ///
    /// <param name="template">The message template.</param>
    /// <param name="parameters">The template parameters.</param>
    /// <param name="language">The resolved language.</param>
    ///
    /// <returns>>The formatted message, or the original template if formatting fails.</returns>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>4.0.0</since>
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
    /// Gets the culture for a language, falling back to the invariant culture.
    /// </summary>
    ///
    /// <param name="language">The language tag.</param>
    ///
    /// <returns>The matching culture, or the invariant culture if none exists.</returns>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>4.0.0</since>
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
