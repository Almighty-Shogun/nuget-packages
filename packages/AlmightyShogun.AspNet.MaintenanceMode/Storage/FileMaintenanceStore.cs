using System.Text.Json;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Logging;

namespace AlmightyShogun.AspNet.MaintenanceMode;

/// <summary>
/// Persists maintenance state in a content-root JSON file and caches reads.
/// </summary>
///
/// <param name="webHostEnvironment">The environment used to locate the state file.</param>
/// <param name="logger"> Logs file and watcher failures. </param>
///
/// <author>Almighty-Shogun</author>
/// <since>4.0.0</since>
internal sealed class FileMaintenanceStore(
    IWebHostEnvironment webHostEnvironment,
    ILogger<FileMaintenanceStore> logger
) : IMaintenanceStore, IDisposable
{
    /// <summary>
    /// JSON serialization settings for the state file.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>4.0.0</since>
    private static readonly JsonSerializerOptions _jsonOptions = new(JsonSerializerDefaults.Web) { WriteIndented = true };

    /// <summary>
    /// Serializes writes and conditional clears.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>4.0.0</since>
    private readonly SemaphoreSlim _writeLock = new(1, 1);

    /// <summary>
    /// Synchronizes watcher initialization and disposal.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>4.0.0</since>
    private readonly Lock _watcherGate = new();

    /// <summary>
    /// The cached state and its generation.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>4.0.0</since>
    private volatile CachedState? _cached;

    /// <summary>
    /// Invalidates cached reads when the file changes.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>4.0.0</since>
    private long _cacheVersion;

    /// <summary>
    /// The generation of the last unverified conditional clear.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    private long _unverifiableVersion = -1;

    /// <summary>
    /// Watches for external changes to the state file.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>4.0.0</since>
    private FileSystemWatcher? _watcher;

    /// <summary>
    /// Indicates that watching is active or the store has been disposed.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>4.0.0</since>
    private bool _watching;

    /// <summary>
    /// The maintenance state file in the content root.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>4.0.0</since>
    private string FilePath => Path.Combine(webHostEnvironment.ContentRootPath, "maintenance.json");

    /// <inheritdoc />
    public async ValueTask<PersistedMaintenanceState?> ReadAsync()
    {
        EnsureWatching();

        long version = Volatile.Read(ref _cacheVersion);

        if (_cached is { } cached && cached.Version == version)
            return cached.State;

        DiskRead read = await ReadFromDiskAsync();

        if (Volatile.Read(ref _cacheVersion) == version)
            _cached = new CachedState(version, read.State);

        return read.State;
    }

    /// <inheritdoc />
    public async Task WriteAsync(PersistedMaintenanceState state)
    {
        await _writeLock.WaitAsync();

        try
        {
            Directory.CreateDirectory(Path.GetDirectoryName(FilePath) ?? webHostEnvironment.ContentRootPath);

            var tempFilePath = $"{FilePath}.{Guid.NewGuid():N}.tmp";

            await using (FileStream stream = File.Create(tempFilePath))
            {
                await JsonSerializer.SerializeAsync(stream, state, _jsonOptions);
            }

            File.Move(tempFilePath, FilePath, true);

            Publish(state);
        }
        finally
        {
            _writeLock.Release();
        }
    }

    /// <inheritdoc />
    public async Task ClearAsync()
    {
        await _writeLock.WaitAsync();

        try
        {
            DeleteFile();

            Publish(null);
        }
        finally
        {
            _writeLock.Release();
        }
    }

    /// <inheritdoc />
    public async Task<MaintenanceClearOutcome> TryClearAsync(Guid expectedRevision)
    {
        if (IsKnownUnverifiable()) return MaintenanceClearOutcome.Unverified;

        await _writeLock.WaitAsync();

        try
        {
            if (IsKnownUnverifiable()) return MaintenanceClearOutcome.Unverified;

            long version = Volatile.Read(ref _cacheVersion);

            DiskRead read = await ReadFromDiskAsync();

            if (read.Outcome is DiskReadOutcome.Unreadable)
            {
                Volatile.Write(ref _unverifiableVersion, version);

                return MaintenanceClearOutcome.Unverified;
            }

            if (read is not { Outcome: DiskReadOutcome.Loaded, State: { } current } || current.Revision != expectedRevision)
            {
                Publish(read.State);

                return MaintenanceClearOutcome.Superseded;
            }

            DeleteFile();

            Publish(null);

            return MaintenanceClearOutcome.Cleared;
        }
        finally
        {
            _writeLock.Release();
        }
    }

    /// <inheritdoc />
    public void Dispose()
    {
        lock (_watcherGate)
        {
            _watching = true;

            _watcher?.Dispose();
            _watcher = null;
        }
    }

    /// <summary>
    /// Publishes a state and advances the cache generation.
    /// </summary>
    ///
    /// <param name="state">The state to cache, or null when no file exists.</param>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>4.0.0</since>
    private void Publish(PersistedMaintenanceState? state) => _cached = new CachedState(Interlocked.Increment(ref _cacheVersion), state);

    /// <summary>
    /// Reports whether a conditional clear already failed to verify this generation.
    /// </summary>
    ///
    /// <returns>True if another disk read should be deferred until invalidation.</returns>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    private bool IsKnownUnverifiable() => Volatile.Read(ref _unverifiableVersion) == Volatile.Read(ref _cacheVersion);

    /// <summary>
    /// Deletes the maintenance state file if it exists.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>4.0.0</since>
    private void DeleteFile()
    {
        if (File.Exists(FilePath))
            File.Delete(FilePath);
    }

    /// <summary>
    /// Reads the file and distinguishes authoritative state from fallback state.
    /// </summary>
    ///
    /// <returns>The read outcome and the state to serve. Only a loaded state is authoritative.</returns>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>4.0.0</since>
    private async Task<DiskRead> ReadFromDiskAsync()
    {
        if (!File.Exists(FilePath)) return new DiskRead(DiskReadOutcome.Missing, null);

        for (var attempt = 0; attempt < 3; attempt++)
        {
            try
            {
                await using FileStream stream = File.OpenRead(FilePath);

                var state = await JsonSerializer.DeserializeAsync<PersistedMaintenanceState>(stream, _jsonOptions);

                return state is null
                    ? new DiskRead(DiskReadOutcome.Corrupt, CreateCorruptState())
                    : new DiskRead(DiskReadOutcome.Loaded, state);
            }
            catch (Exception exception) when (exception is JsonException or NotSupportedException)
            {
                logger.LogError(exception, "The maintenance state file at {FilePath} could not be parsed", FilePath);

                return new DiskRead(DiskReadOutcome.Corrupt, CreateCorruptState());
            }
            catch (Exception exception) when (exception is IOException or UnauthorizedAccessException)
            {
                if (attempt == 2)
                {
                    logger.LogWarning(
                        exception,
                        "The maintenance state file at {FilePath} could not be read; keeping the last known state",
                        FilePath);

                    return new DiskRead(DiskReadOutcome.Unreadable, _cached?.State);
                }

                await Task.Delay(TimeSpan.FromMilliseconds(20 * (attempt + 1)));
            }
        }

        return new DiskRead(DiskReadOutcome.Unreadable, _cached?.State);
    }

    /// <summary>
    /// Creates an enabled, fail-closed state for an invalid file.
    /// </summary>
    ///
    /// <returns>A state that keeps maintenance enabled until the file is corrected.</returns>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>4.0.0</since>
    private static PersistedMaintenanceState CreateCorruptState() => new()
    {
        IsEnabled = true,
        AutoDisableWhenExpired = false,
        EnabledAt = DateTimeOffset.UtcNow,
        Message = "Maintenance file is corrupt, please resolve this."
    };

    /// <summary>
    /// Starts watching the state file, retrying setup on later reads if it fails.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>4.0.0</since>
    private void EnsureWatching()
    {
        lock (_watcherGate)
        {
            if (_watching) return;

            string directory = Path.GetDirectoryName(FilePath) ?? webHostEnvironment.ContentRootPath;

            if (!Directory.Exists(directory)) return;

            try
            {
                FileSystemWatcher watcher = new(directory, "maintenance.json")
                {
                    NotifyFilter = NotifyFilters.LastWrite | NotifyFilters.FileName | NotifyFilters.CreationTime
                };

                watcher.Changed += OnStateFileChanged;
                watcher.Created += OnStateFileChanged;
                watcher.Deleted += OnStateFileChanged;
                watcher.Renamed += OnStateFileChanged;
                watcher.Error += OnWatcherError;

                watcher.EnableRaisingEvents = true;

                _watcher = watcher;
                _watching = true;
            }
            catch (Exception exception) when (exception is IOException or UnauthorizedAccessException or PlatformNotSupportedException)
            {
                logger.LogWarning(
                    exception,
                    "Could not watch {Directory} for maintenance state changes; an out-of-band edit will not be noticed",
                    directory);
            }
        }
    }

    /// <summary>
    /// Invalidates cached state when the file watcher reports an error.
    /// </summary>
    ///
    /// <param name="sender">The file watcher.</param>
    /// <param name="eventArgs">The watcher error.</param>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    private void OnWatcherError(object sender, ErrorEventArgs eventArgs)
    {
        logger.LogWarning(
            eventArgs.GetException(),
            "Maintenance state watcher failed; cached state will be invalidated");

        Interlocked.Increment(ref _cacheVersion);
    }

    /// <summary>
    /// Invalidates cached state after a file change.
    /// </summary>
    ///
    /// <param name="sender">The file watcher.</param>
    /// <param name="eventArgs">The file change event.</param>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>4.0.0</since>
    private void OnStateFileChanged(object sender, FileSystemEventArgs eventArgs) => Interlocked.Increment(ref _cacheVersion);

    /// <summary>
    /// Stores a cached state with its generation.
    /// </summary>
    ///
    /// <param name="Version">The cache generation.</param>
    /// <param name="State">The cached state.</param>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>4.0.0</since>
    private sealed record CachedState(long Version, PersistedMaintenanceState? State);

    /// <summary>
    /// Pairs a disk-read outcome with the state to serve.
    /// </summary>
    ///
    /// <param name="Outcome">The result of reading the file.</param>
    /// <param name="State">The loaded or fallback state.</param>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    private sealed record DiskRead(DiskReadOutcome Outcome, PersistedMaintenanceState? State);
}
