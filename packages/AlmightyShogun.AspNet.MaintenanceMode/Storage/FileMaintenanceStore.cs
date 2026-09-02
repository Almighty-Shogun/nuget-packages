using System.Text.Json;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Logging;

namespace AlmightyShogun.AspNet.MaintenanceMode;

/// <summary>
/// Persists maintenance state to a JSON file in the content root, caching it in memory.
/// </summary>
///
/// <param name="webHostEnvironment">The web host environment used to resolve the content root.</param>
/// <param name="logger">The logger used to report unreadable state files.</param>
///
/// <author>Almighty-Shogun</author>
/// <since>Unreleased</since>
internal sealed class FileMaintenanceStore(
    IWebHostEnvironment webHostEnvironment,
    ILogger<FileMaintenanceStore> logger
) : IMaintenanceStore, IDisposable
{
    /// <summary>
    /// The serializer settings, shared so the file this process writes is the same shape the one it reads expects.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    private static readonly JsonSerializerOptions _jsonOptions = new(JsonSerializerDefaults.Web) { WriteIndented = true };

    /// <summary>
    /// Serializes every write, so two operators opening a window at once cannot interleave into a half-written file, and so a conditional
    /// clear can compare against the file without a write landing in between.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    private readonly SemaphoreSlim _writeLock = new(1, 1);

    /// <summary>
    /// Guards watcher setup and disposal, so the watcher is built exactly once and never after the store is disposed. Taken on every read
    /// through <see cref="EnsureWatching"/>, where it is uncontended once setup has run.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    private readonly Lock _watcherGate = new();

    /// <summary>
    /// The cached state together with the generation it was read under. Every request reads this rather than the disk; without it each one
    /// would cost a file read and a deserialize even with maintenance off.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    private volatile CachedState? _cached;

    /// <summary>
    /// The generation the cache is valid for, incremented by every watcher event and every write. An entry stamped with an earlier
    /// generation is reloaded rather than trusted, which is what stops a read that overlapped a write from publishing the old value.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    private long _cacheVersion;

    /// <summary>
    /// Watches the state file so an out-of-band edit is noticed.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    private FileSystemWatcher? _watcher;

    /// <summary>
    /// Whether watcher setup has already run, or the store has been disposed. Read and written only under <see cref="_watcherGate"/>, so
    /// no second caller can enter setup and no caller can build a watcher after disposal has swept.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    private bool _watching;

    /// <summary>
    /// Resolves the state file's location under the content root, so the file travels with the deployment rather than the working
    /// directory.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    private string FilePath => Path.Combine(webHostEnvironment.ContentRootPath, "maintenance.json");

    /// <inheritdoc />
    public async ValueTask<PersistedMaintenanceState?> ReadAsync()
    {
        EnsureWatching();

        long version = Volatile.Read(ref _cacheVersion);

        if (_cached is { } cached && cached.Version == version)
            return cached.State;

        PersistedMaintenanceState? state = await ReadFromDiskAsync();

        if (Volatile.Read(ref _cacheVersion) == version)
            _cached = new CachedState(version, state);

        return state;
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
    public async Task<bool> TryClearAsync(Guid expectedRevision)
    {
        await _writeLock.WaitAsync();

        try
        {
            PersistedMaintenanceState? current = await ReadFromDiskAsync();

            if (current?.Revision != expectedRevision)
                return false;

            DeleteFile();

            Publish(null);

            return true;
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
    /// Publishes a state the caller has just written to disk, retiring every cache entry read before it.
    /// </summary>
    ///
    /// <param name="state">The state now on disk, or <c>null</c> when the file was deleted.</param>
    ///
    /// <remarks>
    /// The generation is bumped before the entry is stored, so a read that started earlier fails its own version check and reloads instead
    /// of overwriting this. The watcher event this write triggers bumps the generation again, which costs one reload and cannot resurrect
    /// the old value.
    /// </remarks>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    private void Publish(PersistedMaintenanceState? state) => _cached = new CachedState(Interlocked.Increment(ref _cacheVersion), state);

    /// <summary>
    /// Deletes the state file when it is there, which is what closing a window amounts to on disk.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    private void DeleteFile()
    {
        if (File.Exists(FilePath))
            File.Delete(FilePath);
    }

    /// <summary>
    /// Reads and parses the state file, distinguishing a wrong file from an unreadable one.
    /// </summary>
    ///
    /// <returns>
    /// The persisted state; <c>null</c> when the file does not exist; the fail-closed state when it exists but cannot be parsed; and the
    /// last cached value, which may itself be <c>null</c>, when three read attempts all failed.
    /// </returns>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    private async Task<PersistedMaintenanceState?> ReadFromDiskAsync()
    {
        if (!File.Exists(FilePath)) return null;

        for (var attempt = 0; attempt < 3; attempt++)
        {
            try
            {
                await using FileStream stream = File.OpenRead(FilePath);

                return await JsonSerializer.DeserializeAsync<PersistedMaintenanceState>(stream, _jsonOptions) ?? CreateCorruptState();
            }
            catch (Exception exception) when (exception is JsonException or NotSupportedException)
            {
                logger.LogError(exception, "The maintenance state file at {FilePath} could not be parsed", FilePath);

                return CreateCorruptState();
            }
            catch (Exception exception) when (exception is IOException or UnauthorizedAccessException)
            {
                if (attempt == 2)
                {
                    logger.LogWarning(
                        exception,
                        "The maintenance state file at {FilePath} could not be read; keeping the last known state",
                        FilePath
                    );

                    return _cached?.State;
                }

                await Task.Delay(TimeSpan.FromMilliseconds(20 * (attempt + 1)));
            }
        }

        return _cached?.State;
    }

    /// <summary>
    /// Builds the fail-closed state used when the file exists but cannot be parsed.
    /// </summary>
    ///
    /// <returns>An enabled state that keeps maintenance active until the file is fixed.</returns>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    private static PersistedMaintenanceState CreateCorruptState() => new()
    {
        IsEnabled = true,
        AutoDisableWhenExpired = false,
        EnabledAt = DateTimeOffset.UtcNow,
        Message = "Maintenance file is corrupt, please resolve this."
    };

    /// <summary>
    /// Starts watching the content root for changes to the state file, the first time state is read.
    /// </summary>
    ///
    /// <remarks>
    /// The flag is checked inside the lock rather than before it, because two requests arriving together would otherwise each build a
    /// watcher and only one would be reachable to dispose. Disposal takes the same lock and sets the same flag, so a read arriving during
    /// shutdown cannot create a watcher that nothing will dispose.
    /// </remarks>
    ///
    /// <remarks>
    /// The watcher is armed only once its handlers are attached. Setting <c>EnableRaisingEvents</c> in the object initializer instead would
    /// leave a window in which an edit raises an event that nothing is subscribed to, and the cache would keep serving the old state.
    /// </remarks>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    private void EnsureWatching()
    {
        lock (_watcherGate)
        {
            if (_watching) return;

            _watching = true;

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

                watcher.EnableRaisingEvents = true;

                _watcher = watcher;
            }
            catch (Exception exception) when (exception is IOException or UnauthorizedAccessException or PlatformNotSupportedException)
            {
                logger.LogWarning(
                    exception,
                    "Could not watch {Directory} for maintenance state changes; an out-of-band edit will not be noticed",
                    directory
                );
            }
        }
    }

    /// <summary>
    /// Invalidates the cache after an out-of-band edit, so a file changed by hand takes effect without a restart.
    /// </summary>
    ///
    /// <param name="sender">The watcher that raised the change. Unused: any change to the file invalidates the whole cache.</param>
    /// <param name="eventArgs">The file system event arguments.</param>
    ///
    /// <remarks>
    /// The generation is bumped rather than the entry removed. A read already in flight was started under the old generation and will
    /// refuse to store its result, so an edit cannot be overtaken by a read that began before it.
    /// </remarks>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    private void OnStateFileChanged(object sender, FileSystemEventArgs eventArgs) => Interlocked.Increment(ref _cacheVersion);

    /// <summary>
    /// Wraps the cached value with the generation it was read under, so a cached <c>null</c> is distinguishable from nothing cached and a
    /// superseded entry is recognizable without the invalidating side having to find and remove it.
    /// </summary>
    ///
    /// <param name="Version">The cache generation the value was read under.</param>
    /// <param name="State">The cached state.</param>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    private sealed record CachedState(long Version, PersistedMaintenanceState? State);
}
