using System.Text.Json;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using System.Collections.Concurrent;

namespace AlmightyShogun.AspNet.Utils;

/// <summary>
/// Resolves HTTP messages from JSON message files in the application message directories.
/// </summary>
///
/// <param name="configuration">The application configuration used to read the default message language.</param>
/// <param name="languageProvider">The provider used to resolve the active language.</param>
/// <param name="webHostEnvironment">The optional web host environment used to resolve the content root.</param>
///
/// <author>Almighty-Shogun</author>
/// <since>Unreleased</since>
internal sealed class JsonMessageResolver(
    IConfiguration configuration,
    ILanguageProvider languageProvider,
    IWebHostEnvironment? webHostEnvironment = null) : IMessageResolver
{
    /// <summary>
    /// The directory name that contains language-specific message files.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    private const string _messagesDirectoryName = "messages";

    /// <summary>
    /// The cached flattened message dictionaries keyed by language.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    private readonly ConcurrentDictionary<string, IReadOnlyDictionary<string, string>> _messages = new(StringComparer.OrdinalIgnoreCase);

    /// <inheritdoc />
    public string Resolve(string key) => Resolve(key, []);

    /// <inheritdoc />
    public string Resolve(string key, IReadOnlyList<object?> parameters)
    {
        foreach (string language in GetLanguageCandidates())
        {
            IReadOnlyDictionary<string, string> messages = _messages.GetOrAdd(language, LoadMessages);

            if (!messages.TryGetValue(key, out string? template))
            {
                continue;
            }

            languageProvider.SetContentLanguage(language);

            return Format(template, parameters);
        }

        return key;
    }

    /// <summary>
    /// Builds the language fallback chain for the current request.
    /// </summary>
    ///
    /// <returns>The requested language, its neutral language, and the configured default language when applicable.</returns>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    private IEnumerable<string> GetLanguageCandidates()
    {
        string requestedLanguage = languageProvider.GetLanguage();

        yield return requestedLanguage;

        string? neutralLanguage = GetNeutralLanguage(requestedLanguage);

        if (neutralLanguage is not null && !neutralLanguage.Equals(requestedLanguage, StringComparison.OrdinalIgnoreCase))
        {
            yield return neutralLanguage;
        }

        string defaultLanguage = HttpMessageConfiguration.GetDefaultLanguage(configuration);

        if (!defaultLanguage.Equals(requestedLanguage, StringComparison.OrdinalIgnoreCase))
        {
            yield return defaultLanguage;
        }
    }

    /// <summary>
    /// Loads and flattens all message files for a language.
    /// </summary>
    ///
    /// <param name="language">The language to load.</param>
    ///
    /// <returns>The flattened message dictionary for the language.</returns>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    private IReadOnlyDictionary<string, string> LoadMessages(string language)
    {
        Dictionary<string, string> messages = new(StringComparer.OrdinalIgnoreCase);

        foreach (string filePath in ResolveFilePaths(language))
        {
            using FileStream stream = File.OpenRead(filePath);
            using var document = JsonDocument.Parse(stream);

            FlattenMessageFile(filePath, document.RootElement, messages);
        }

        return messages;
    }

    /// <summary>
    /// Resolves all JSON message files for a language across the configured search roots.
    /// </summary>
    ///
    /// <param name="language">The language to resolve.</param>
    ///
    /// <returns>The distinct message file paths for the language.</returns>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    private IEnumerable<string> ResolveFilePaths(string language)
    {
        HashSet<string> yielded = new(StringComparer.OrdinalIgnoreCase);

        foreach (string root in GetSearchRoots())
        {
            string directory = Path.Combine(root, _messagesDirectoryName, language);

            if (!Directory.Exists(directory))
            {
                continue;
            }

            foreach (string filePath in Directory.EnumerateFiles(directory, "*.json").Order(StringComparer.OrdinalIgnoreCase))
            {
                if (yielded.Add(filePath))
                {
                    yield return filePath;
                }
            }
        }
    }

    /// <summary>
    /// Resolves the directories that may contain message files.
    /// </summary>
    ///
    /// <returns>The candidate message search roots.</returns>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    private IEnumerable<string> GetSearchRoots()
    {
        if (webHostEnvironment?.ContentRootPath is not null)
        {
            yield return webHostEnvironment.ContentRootPath;
        }

        yield return AppContext.BaseDirectory;
        yield return Directory.GetCurrentDirectory();
    }

    /// <summary>
    /// Flattens a single message file into the message dictionary using the file name as the root key.
    /// </summary>
    ///
    /// <param name="filePath">The message file path.</param>
    /// <param name="element">The root JSON element.</param>
    /// <param name="messages">The message dictionary to populate.</param>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    private static void FlattenMessageFile(string filePath, JsonElement element, Dictionary<string, string> messages)
    {
        string group = Path.GetFileNameWithoutExtension(filePath);

        foreach (JsonProperty property in element.EnumerateObject())
        {
            string key = property.Name.Equals(group, StringComparison.OrdinalIgnoreCase)
                ? group : $"{group}.{property.Name}";

            Flatten(key, property.Value, messages);
        }
    }

    /// <summary>
    /// Recursively flattens nested JSON message objects into dot-separated message keys.
    /// </summary>
    ///
    /// <param name="prefix">The current message key prefix.</param>
    /// <param name="element">The JSON element to flatten.</param>
    /// <param name="messages">The message dictionary to populate.</param>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    private static void Flatten(string prefix, JsonElement element, Dictionary<string, string> messages)
    {
        if (element.ValueKind == JsonValueKind.String)
        {
            string key = NormalizeMessageKey(prefix);
            messages[key] = element.GetString() ?? key;

            return;
        }

        if (element.ValueKind != JsonValueKind.Object)
        {
            return;
        }

        foreach (JsonProperty property in element.EnumerateObject())
        {
            Flatten($"{prefix}.{property.Name}", property.Value, messages);
        }
    }

    /// <summary>
    /// Normalizes a flattened message key by removing a trailing default suffix.
    /// </summary>
    ///
    /// <param name="key">The message key to normalize.</param>
    ///
    /// <returns>The normalized message key.</returns>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    private static string NormalizeMessageKey(string key)
    {
        const string defaultSuffix = ".default";

        return key.EndsWith(defaultSuffix, StringComparison.OrdinalIgnoreCase)
            ? key[..^defaultSuffix.Length] : key;
    }

    /// <summary>
    /// Resolves the neutral language for a culture-specific language tag.
    /// </summary>
    ///
    /// <param name="language">The requested language tag.</param>
    ///
    /// <returns>The neutral language when available; otherwise, <c>null</c>.</returns>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    private static string? GetNeutralLanguage(string language)
    {
        int separatorIndex = language.IndexOf('-', StringComparison.Ordinal);

        return separatorIndex <= 0 ? null : language[..separatorIndex];
    }

    /// <summary>
    /// Formats a message template with the supplied parameters.
    /// </summary>
    ///
    /// <param name="template">The message template.</param>
    /// <param name="parameters">The template parameters.</param>
    ///
    /// <returns>The formatted message, or the original template when formatting fails.</returns>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    private static string Format(string template, IReadOnlyList<object?> parameters)
    {
        if (parameters.Count == 0)
        {
            return template;
        }

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
