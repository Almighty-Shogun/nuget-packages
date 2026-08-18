using System.Text.Json;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Collections.Concurrent;

namespace AlmightyShogun.AspNet.Utils;

/// <summary>
/// Loads the JSON message files for a language, flattens them to dot-separated keys, and caches the result for the life
/// of the process unless automatic reload is enabled.
/// </summary>
///
/// <param name="localizationOptions">The settings deciding whether the message directories are watched for changes.</param>
/// <param name="logger">
/// The logger an unreadable or malformed message file is reported on. Such a file is skipped rather than fatal, so this
/// warning is the only sign its messages are absent.
/// </param>
/// <param name="webHostEnvironment">
/// The environment supplying the content root, which is searched ahead of the other roots. Optional so the store can be
/// resolved outside a web host, such as in a test or a console process.
/// </param>
///
/// <remarks>
/// A language whose directory does not exist is deliberately not cached. Its absence may simply mean the deployment has
/// not written it yet, and caching an empty result would make that permanent for the rest of the process.
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
    /// Concurrent because resolution happens on request threads with no lock around the lookup.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    private readonly ConcurrentDictionary<string, IReadOnlyDictionary<string, string>> _messages = new(StringComparer.OrdinalIgnoreCase);

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
    /// Whether setup has already run. Checked before taking the lock so the common path costs a field read, and again
    /// inside it because the first check is not synchronized.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    private bool _watching;

    /// <inheritdoc />
    public IReadOnlyDictionary<string, string> GetMessages(string language)
    {
        StartWatchingIfEnabled();

        if (_messages.TryGetValue(language, out IReadOnlyDictionary<string, string>? cached))
            return cached;

        (IReadOnlyDictionary<string, string> messages, bool directoryExisted) = LoadMessages(language);

        return directoryExisted ? _messages.GetOrAdd(language, messages) : messages;
    }

    /// <inheritdoc />
    public void Dispose()
    {
        foreach (FileSystemWatcher watcher in _watchers)
            watcher.Dispose();

        _watchers.Clear();
    }

    /// <summary>
    /// Loads and flattens every message file defined for a language, across all search roots.
    /// </summary>
    ///
    /// <param name="language">The exact language tag, used as the directory name under each search root.</param>
    ///
    /// <returns>
    /// The merged messages from every search root, and whether any root actually held a directory for the language. The
    /// flag is what the caller uses to decide against caching a result that only looks empty.
    /// </returns>
    ///
    /// <remarks>
    /// Roots are merged in search order and files within a root in name order, with later entries overwriting earlier
    /// ones. A later root therefore overrides the content root, which is what lets a deployment patch a single key.
    /// </remarks>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    private (IReadOnlyDictionary<string, string> Messages, bool DirectoryExisted) LoadMessages(string language)
    {
        var directoryExisted = false;

        Dictionary<string, string> messages = new(StringComparer.OrdinalIgnoreCase);

        foreach (string root in GetSearchRoots())
        {
            string directory = Path.Combine(root, MessagesDirectoryName, language);

            if (!Directory.Exists(directory)) continue;

            directoryExisted = true;

            IEnumerable<string> localizationFiles = Directory.EnumerateFiles(directory, "*.json");

            foreach (string filePath in localizationFiles.Order(StringComparer.OrdinalIgnoreCase))
                LoadFile(filePath, messages);
        }

        return (messages, directoryExisted);
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
    /// current working directory. All three are searched; they are not alternatives, and duplicates are not removed.
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
    /// Clears the cache so the next resolution reloads from disk.
    /// </summary>
    ///
    /// <param name="sender">The watcher that raised the event. Unused: every root invalidates the whole cache.</param>
    /// <param name="eventArgs">
    /// What changed. Unused for the same reason, and because an editor saving a file commonly raises several events for
    /// one edit, which clearing everything absorbs without needing to be debounced.
    /// </param>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    private void OnMessageFileChanged(object sender, FileSystemEventArgs eventArgs) => _messages.Clear();

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
