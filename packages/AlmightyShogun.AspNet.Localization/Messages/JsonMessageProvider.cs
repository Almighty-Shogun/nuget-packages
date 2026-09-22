using System.Text.Json;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Collections.Concurrent;
using System.Collections.ObjectModel;
using Microsoft.Extensions.Caching.Memory;

namespace AlmightyShogun.AspNet.Localization;

/// <summary>
/// Loads and caches localized messages from JSON files.
/// </summary>
///
/// <param name="localizationOptions">The localization settings.</param>
/// <param name="logger"> The logger. </param>
/// <param name="webHostEnvironment"> The web host environment, when available. </param>
///
/// <author>Almighty-Shogun</author>
/// <since>4.0.0</since>
internal sealed class JsonMessageProvider(
    IOptions<LocalizationSettings> localizationOptions,
    ILogger<JsonMessageProvider> logger,
    IWebHostEnvironment? webHostEnvironment = null
) : IMessageProvider, IDisposable
{
    private const string _messagesDirectoryName = "messages";
    
    private const int _missCacheSizeLimit = 1024;
    
    private static readonly MemoryCacheEntryOptions _missCacheEntryOptions = new() { Size = 1 };
    
    private readonly ConcurrentDictionary<string, CachedMessages> _messages = new(StringComparer.OrdinalIgnoreCase);
    
    private readonly MemoryCache _misses = new(new MemoryCacheOptions { SizeLimit = _missCacheSizeLimit });
    
    private long _cacheVersion;
    
    private readonly List<FileSystemWatcher> _watchers = [];
    
    private readonly Lock _watcherGate = new();
    
    private bool _watching;

    /// <inheritdoc />
    public IReadOnlyDictionary<string, string> GetMessages(string language)
    {
        if (!LanguageTag.IsValid(language))
        {
            logger.LogWarning("Ignored malformed language tag {Language}", language);

            return ReadOnlyDictionary<string, string>.Empty;
        }

        StartWatchingIfEnabled();

        long version = Volatile.Read(ref _cacheVersion);

        if (_messages.TryGetValue(language, out CachedMessages? cached) && cached.Version == version)
            return cached.Messages;

        string missKey = language.ToLowerInvariant();

        if (IsCachedMiss(missKey, version))
            return ReadOnlyDictionary<string, string>.Empty;

        IReadOnlyDictionary<string, string> messages = LoadMessages(language);

        if (messages.Count == 0)
        {
            if (Volatile.Read(ref _cacheVersion) == version)
                CacheMiss(missKey, version);

            return ReadOnlyDictionary<string, string>.Empty;
        }

        if (Volatile.Read(ref _cacheVersion) == version)
            _messages[language] = new CachedMessages(version, messages);

        return messages;
    }

    /// <inheritdoc />
    public void Dispose()
    {
        lock (_watcherGate)
        {
            _watching = true;

            foreach (FileSystemWatcher watcher in _watchers)
                watcher.Dispose();

            _watchers.Clear();
        }

        _misses.Dispose();
    }

    /// <summary>
    /// Determines whether a language is cached as having no messages.
    /// </summary>
    ///
    /// <param name="missKey">The normalized language key.</param>
    /// <param name="version">The current cache version.</param>
    ///
    /// <returns>
    /// <c>true</c> if the language is cached as missing for the specified version; otherwise, <c>false</c>.
    /// </returns>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    private bool IsCachedMiss(string missKey, long version)
    {
        try
        {
            return _misses.TryGetValue(missKey, out long missVersion) && missVersion == version;
        }
        catch (ObjectDisposedException)
        {
            return false;
        }
    }

    /// <summary>
    /// Caches a language as having no messages.
    /// </summary>
    ///
    /// <param name="missKey">The normalized language key.</param>
    /// <param name="version">The current cache version.</param>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    private void CacheMiss(string missKey, long version)
    {
        try
        {
            _misses.Set(missKey, version, _missCacheEntryOptions);
        }
        catch (ObjectDisposedException) { }
    }

    /// <summary>
    /// Loads messages for a language from the configured search roots.
    /// </summary>
    ///
    /// <param name="language">The language to load.</param>
    ///
    /// <returns>The loaded messages.</returns>
    ///
    /// <remarks>
    /// Language directories are matched case-insensitively.
    /// When multiple roots define the same key, the first root takes precedence.
    /// </remarks>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>4.0.0</since>
    private IReadOnlyDictionary<string, string> LoadMessages(string language)
    {
        Dictionary<string, string> messages = new(StringComparer.OrdinalIgnoreCase);

        foreach (string root in GetSearchRoots())
        {
            string messagesDirectory = Path.Combine(root, _messagesDirectoryName);

            if (!Directory.Exists(messagesDirectory)) continue;

            string? directory = Directory
                .EnumerateDirectories(messagesDirectory)
                .FirstOrDefault(path =>
                    Path.GetFileName(path).Equals(
                        language,
                        StringComparison.OrdinalIgnoreCase));

            if (directory is null)
                continue;

            Dictionary<string, string> fromRoot = new(StringComparer.OrdinalIgnoreCase);

            IEnumerable<string> localizationFiles = Directory.EnumerateFiles(directory, "*.json");

            foreach (string filePath in localizationFiles.Order(StringComparer.OrdinalIgnoreCase))
                LoadFile(filePath, fromRoot);

            foreach ((string key, string message) in fromRoot)
                messages.TryAdd(key, message);
        }

        return messages;
    }

    /// <summary>
    /// Loads messages from a JSON file.
    /// </summary>
    ///
    /// <param name="filePath">The message file.</param>
    /// <param name="messages">The destination dictionary.</param>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>4.0.0</since>
    private void LoadFile(string filePath, Dictionary<string, string> messages)
    {
        try
        {
            using FileStream stream = File.OpenRead(filePath);
            using JsonDocument document = JsonDocument.Parse(stream);

            FlattenMessageFile(filePath, document.RootElement, messages);
        }
        catch (Exception exception) when (exception is JsonException or IOException or UnauthorizedAccessException)
        {
            logger.LogWarning(exception, "Skipped unreadable message file {MessageFile}", filePath);
        }
    }

    /// <summary>
    /// Enumerates distinct message search roots.
    /// </summary>
    ///
    /// <returns>The message search roots in precedence order. </returns>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>4.0.0</since>
    private IEnumerable<string> GetSearchRoots()
    {
        StringComparer comparer = OperatingSystem.IsWindows()
            ? StringComparer.OrdinalIgnoreCase
            : StringComparer.Ordinal;

        HashSet<string> seen = new(comparer);

        foreach (string root in EnumerateRoots())
            if (seen.Add(Path.TrimEndingDirectorySeparator(Path.GetFullPath(root))))
                yield return root;
    }

    /// <summary>
    /// Enumerates candidate message search roots.
    /// </summary>
    ///
    /// <returns>The candidate roots in precedence order.</returns>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>4.0.0</since>
    private IEnumerable<string> EnumerateRoots()
    {
        if (webHostEnvironment?.ContentRootPath is not null)
            yield return webHostEnvironment.ContentRootPath;

        yield return AppContext.BaseDirectory;
        yield return Directory.GetCurrentDirectory();
    }

    /// <summary>
    /// Starts message file watchers when automatic reload is enabled.
    /// </summary>
    ///
    /// <remarks>
    /// Only message directories that already exist are watched.
    /// </remarks>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>4.0.0</since>
    private void StartWatchingIfEnabled()
    {
        if (_watching || !localizationOptions.Value.AutomaticReload) return;

        lock (_watcherGate)
        {
            if (_watching) return;

            _watching = true;

            List<FileSystemWatcher> watchers = [];

            try
            {
                foreach (string root in GetSearchRoots())
                {
                    string directory = Path.Combine(root, _messagesDirectoryName);

                    if (!Directory.Exists(directory)) continue;

                    FileSystemWatcher watcher = new(directory, "*.json")
                    {
                        IncludeSubdirectories = true,
                        NotifyFilter = NotifyFilters.LastWrite | NotifyFilters.FileName | NotifyFilters.CreationTime
                    };

                    watcher.Changed += OnMessageFileChanged;
                    watcher.Created += OnMessageFileChanged;
                    watcher.Deleted += OnMessageFileChanged;
                    watcher.Renamed += OnMessageFileChanged;
                    watcher.Error += OnMessageWatcherError;

                    watchers.Add(watcher);
                    
                    watcher.EnableRaisingEvents = true;
                }

                _watchers.AddRange(watchers);
                _watching = true;
            }
            catch
            {
                foreach (FileSystemWatcher watcher in watchers)
                    watcher.Dispose();
                
                throw;
            }
        }
    }
    
    private void OnMessageFileChanged(object sender, FileSystemEventArgs eventArgs) => Interlocked.Increment(ref _cacheVersion);

    private void OnMessageWatcherError(
        object sender,
        ErrorEventArgs eventArgs)
    {
        logger.LogWarning(
            eventArgs.GetException(),
            "Message file watcher encountered an error");

        Interlocked.Increment(ref _cacheVersion);
    }

    /// <summary>
    /// Flattens a message file into prefixed message keys.
    /// </summary>
    ///
    /// <param name="filePath">The message file.</param>
    /// <param name="element">The JSON document root.</param>
    /// <param name="messages">The destination dictionary.</param>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>4.0.0</since>
    private static void FlattenMessageFile(string filePath, JsonElement element, Dictionary<string, string> messages)
    {
        if (element.ValueKind != JsonValueKind.Object) return;

        string group = Path.GetFileNameWithoutExtension(filePath);

        foreach (JsonProperty property in element.EnumerateObject())
        {
            string key = property.Name.Equals(group, StringComparison.OrdinalIgnoreCase) ? group : $"{group}.{property.Name}";

            Flatten(key, property.Value, messages);
        }
    }

    /// <summary>
    /// Flattens nested message objects into dot-separated keys.
    /// </summary>
    ///
    /// <param name="prefix">The current message key.</param>
    /// <param name="element">The JSON element.</param>
    /// <param name="messages">The destination dictionary.</param>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>4.0.0</since>
    private static void Flatten(string prefix, JsonElement element, Dictionary<string, string> messages)
    {
        if (element.ValueKind == JsonValueKind.String)
        {
            string key = NormalizeMessageKey(prefix);
            messages[key] = element.GetString() ?? key;

            return;
        }

        if (element.ValueKind != JsonValueKind.Object) return;

        foreach (JsonProperty property in element.EnumerateObject())
            Flatten($"{prefix}.{property.Name}", property.Value, messages);
    }

    /// <summary>
    /// Removes a trailing <c>.default</c> segment from a message key.
    /// </summary>
    ///
    /// <param name="key">The message key.</param>
    ///
    /// <returns>The normalized message key.</returns>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>4.0.0</since>
    private static string NormalizeMessageKey(string key)
    {
        const string defaultSuffix = ".default";

        return key.EndsWith(defaultSuffix, StringComparison.OrdinalIgnoreCase) ? key[..^defaultSuffix.Length] : key;
    }
}
