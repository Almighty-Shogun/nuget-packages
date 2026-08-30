using System.Text.Json;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Collections.Concurrent;
using System.Collections.ObjectModel;

namespace AlmightyShogun.AspNet.Localization;

/// <summary>
/// Loads the JSON message files for a language, flattens them to dot-separated keys, and caches the result for the life
/// of the process unless automatic reload is enabled.
/// </summary>
///
/// <param name="localizationOptions">The settings deciding whether the message directories are watched for changes.</param>
/// <param name="logger">
/// The logger a skipped message file and a rejected language tag are reported on. Neither is fatal, so these warnings
/// are the only sign that messages are missing.
/// </param>
/// <param name="webHostEnvironment">
/// The environment supplying the content root, which is searched ahead of the other roots. Optional so the store can be
/// resolved outside a web host, such as in a test or a console process.
/// </param>
///
/// <remarks>
/// Every lookup is cached, a language with no directory included, so an <c>Accept-Language</c> header naming languages
/// the deployment does not have cannot make each request re-walk the filesystem. That means a directory added while the
/// process runs is picked up only with <c>AutomaticReload</c> on, which is the same rule already governing file edits.
/// A tag that is not well-formed is rejected outright, since the value is used as a directory name.
/// </remarks>
///
/// <author>Almighty-Shogun</author>
/// <since>Unreleased</since>
internal sealed class JsonMessageStore(
    IOptions<LocalizationSettings> localizationOptions,
    ILogger<JsonMessageStore> logger,
    IWebHostEnvironment? webHostEnvironment = null
) : IMessageStore, IDisposable
{
    /// <summary>
    /// The directory each search root is expected to contain, holding one subdirectory per language tag.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    private const string MessagesDirectoryName = "messages";

    /// <summary>
    /// The flattened messages per language, keyed case-insensitively so <c>NL-be</c> and <c>nl-BE</c> share an entry.
    /// Concurrent because resolution happens on request threads with no lock around the lookup. Each entry carries the
    /// generation its files were read under, which is what makes a load that overlapped a file change detectable
    /// instead of silently authoritative.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    private readonly ConcurrentDictionary<string, CachedMessages> _messages = new(StringComparer.OrdinalIgnoreCase);

    /// <summary>
    /// The generation the cache is currently valid for, incremented by every watcher event. An entry stamped with an
    /// earlier generation is ignored and reloaded rather than evicted, so invalidation costs one interlocked increment
    /// and never has to race the readers it invalidates.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    private long _cacheVersion;

    /// <summary>
    /// One watcher per search root that exists, held only to keep them alive and to dispose them later. Empty when
    /// automatic reload is off, and never rebuilt afterward.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    private readonly List<FileSystemWatcher> _watchers = [];

    /// <summary>
    /// Serializes watcher creation. Only the setup path takes it; message lookups stay lock-free, since the cache is
    /// concurrent on its own.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    private readonly Lock _watcherGate = new();

    /// <summary>
    /// Whether setup has already run, or the store has been disposed. Checked before taking the lock so the common path
    /// costs a field read, and again inside it because the first check is not synchronized. Disposal sets it too, so a
    /// lookup arriving during shutdown cannot create watchers that nothing will dispose.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
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

        IReadOnlyDictionary<string, string> messages = LoadMessages(language);

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
    }

    /// <summary>
    /// Loads and flattens every message file defined for a language, across all search roots.
    /// </summary>
    ///
    /// <param name="language">The exact language tag, used as the directory name under each search root.</param>
    ///
    /// <returns>
    /// The merged messages from every search root, empty when no root holds a directory for the language.
    /// </returns>
    ///
    /// <remarks>
    /// The first root to define a key keeps it, so the content root wins over the output and working directories rather
    /// than the other way round: the application's own files should not be displaceable by whatever directory the
    /// process happens to have been started from. Within one root, files are read in name order and later files do
    /// overwrite earlier ones, which only matters when two files in the same directory define the same key.
    /// </remarks>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    private IReadOnlyDictionary<string, string> LoadMessages(string language)
    {
        Dictionary<string, string> messages = new(StringComparer.OrdinalIgnoreCase);

        foreach (string root in GetSearchRoots())
        {
            string directory = Path.Combine(root, MessagesDirectoryName, language);

            if (!Directory.Exists(directory)) continue;

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
    /// Loads a single message file into the dictionary, skipping it when it cannot be read or parsed.
    /// </summary>
    ///
    /// <param name="filePath">The file to read. Its name becomes the first segment of every key it contributes.</param>
    /// <param name="messages">
    /// The dictionary being built, mutated in place. Entries already added by an earlier file are overwritten on a key
    /// collision rather than merged.
    /// </param>
    ///
    /// <remarks>
    /// A malformed or unreadable file is logged and skipped, so one bad file costs its own messages instead of every
    /// message in the language. Anything outside JSON, IO, and access failures still propagates.
    /// </remarks>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
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
    /// Resolves the directories that may contain a <c>messages</c> folder.
    /// </summary>
    ///
    /// <returns>
    /// The content root when a web host supplied one, then the directory the assemblies were loaded from, then the
    /// current working directory. All are searched rather than being alternatives, and a root repeated between them is
    /// yielded once, since the content root and the working directory are commonly the same path.
    /// </returns>
    ///
    /// <remarks>
    /// The base directory is included because message files copied to the output folder do not sit under the content
    /// root, and the working directory because a process launched from elsewhere would otherwise find neither.
    /// </remarks>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    private IEnumerable<string> GetSearchRoots()
    {
        HashSet<string> seen = new(StringComparer.OrdinalIgnoreCase);

        foreach (string root in EnumerateRoots())
            if (seen.Add(Path.TrimEndingDirectorySeparator(Path.GetFullPath(root))))
                yield return root;
    }

    /// <summary>
    /// Yields the candidate roots in trust order, before duplicates between them are removed.
    /// </summary>
    ///
    /// <returns>
    /// The content root first, then the output directory, then the working directory. Outside a web host the first is
    /// absent, which is what the optional environment allows for.
    /// </returns>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    private IEnumerable<string> EnumerateRoots()
    {
        if (webHostEnvironment?.ContentRootPath is not null)
            yield return webHostEnvironment.ContentRootPath;

        yield return AppContext.BaseDirectory;
        yield return Directory.GetCurrentDirectory();
    }

    /// <summary>
    /// Starts watching the message directories the first time messages are requested, when automatic reload is enabled.
    /// </summary>
    ///
    /// <remarks>
    /// Deferred to first use rather than done at construction so an application that never resolves a message pays for
    /// no watchers. A root created after this runs is never picked up, since setup happens exactly once.
    /// </remarks>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    private void StartWatchingIfEnabled()
    {
        if (_watching || !localizationOptions.Value.AutomaticReload) return;

        lock (_watcherGate)
        {
            if (_watching) return;

            _watching = true;

            foreach (string root in GetSearchRoots())
            {
                string directory = Path.Combine(root, MessagesDirectoryName);

                if (!Directory.Exists(directory)) continue;

                FileSystemWatcher watcher = new(directory, "*.json")
                {
                    EnableRaisingEvents = true,
                    IncludeSubdirectories = true,
                    NotifyFilter = NotifyFilters.LastWrite | NotifyFilters.FileName | NotifyFilters.CreationTime
                };

                watcher.Changed += OnMessageFileChanged;
                watcher.Created += OnMessageFileChanged;
                watcher.Deleted += OnMessageFileChanged;
                watcher.Renamed += OnMessageFileChanged;

                _watchers.Add(watcher);
            }
        }
    }

    /// <summary>
    /// Retires every cached language so the next resolution reloads from disk.
    /// </summary>
    ///
    /// <param name="sender">The watcher that raised the event. Unused: every root invalidates the whole cache.</param>
    /// <param name="eventArgs">
    /// What changed. Unused for the same reason, and because an editor saving a file commonly raises several events for
    /// one edit, which retiring everything absorbs without needing to be debounced.
    /// </param>
    ///
    /// <remarks>
    /// The generation is bumped rather than the entries removed. A load already in flight was started under the old
    /// generation and will refuse to store its result, so a reader cannot publish a snapshot taken before this fired.
    /// </remarks>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    private void OnMessageFileChanged(object sender, FileSystemEventArgs eventArgs) => Interlocked.Increment(ref _cacheVersion);

    /// <summary>
    /// Flattens one parsed message file, prefixing every key with the file name.
    /// </summary>
    ///
    /// <param name="filePath">The file the element came from. Only its name without extension is used, as the prefix.</param>
    /// <param name="element">
    /// The document root. Expected to be an object; anything else contributes nothing, since only properties are walked.
    /// </param>
    /// <param name="messages">The dictionary being built, mutated in place.</param>
    ///
    /// <remarks>
    /// A top-level property matching the file name is not repeated in the key, so <c>errors.json</c> holding an
    /// <c>errors</c> object yields <c>errors.notFound</c> rather than <c>errors.errors.notFound</c>.
    /// </remarks>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    private static void FlattenMessageFile(string filePath, JsonElement element, Dictionary<string, string> messages)
    {
        string group = Path.GetFileNameWithoutExtension(filePath);

        foreach (JsonProperty property in element.EnumerateObject())
        {
            string key = property.Name.Equals(group, StringComparison.OrdinalIgnoreCase) ? group : $"{group}.{property.Name}";

            Flatten(key, property.Value, messages);
        }
    }

    /// <summary>
    /// Recursively flattens nested message objects into dot-separated keys.
    /// </summary>
    ///
    /// <param name="prefix">The key built from the path walked so far, which becomes the full key at a string leaf.</param>
    /// <param name="element">
    /// The node to walk. Strings are recorded and objects descended into; every other kind, arrays and numbers included,
    /// is dropped, because a message is always a string.
    /// </param>
    /// <param name="messages">The dictionary being built, mutated in place.</param>
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

        if (element.ValueKind != JsonValueKind.Object) return;

        foreach (JsonProperty property in element.EnumerateObject())
            Flatten($"{prefix}.{property.Name}", property.Value, messages);
    }

    /// <summary>
    /// Strips a trailing <c>.default</c> from a flattened key.
    /// </summary>
    ///
    /// <param name="key">The fully built key of a string leaf.</param>
    ///
    /// <returns>The key without its <c>.default</c> suffix, or the key unchanged when it has none.</returns>
    ///
    /// <remarks>
    /// This is what lets a key be both a message and a group: <c>password.default</c> and <c>password.tooShort</c> can
    /// live side by side, with the first resolving under the bare <c>password</c> key.
    /// </remarks>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    private static string NormalizeMessageKey(string key)
    {
        const string defaultSuffix = ".default";

        return key.EndsWith(defaultSuffix, StringComparison.OrdinalIgnoreCase) ? key[..^defaultSuffix.Length] : key;
    }
}
