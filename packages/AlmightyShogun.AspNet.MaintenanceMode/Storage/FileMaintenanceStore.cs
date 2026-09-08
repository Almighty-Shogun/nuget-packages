using System.Text.Json;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Logging;

namespace AlmightyShogun.AspNet.MaintenanceMode;

/// <summary>
/// Persists maintenance state to a JSON file in the content root, caching it in memory.
/// </summary>
///
/// <param name="webHostEnvironment">The web host environment used to resolve the content root.</param>
/// <param name="logger">
/// The logger used to report a state file that could not be parsed or could not be opened, and a watcher that could not be set up.
/// </param>
///
/// <author>Almighty-Shogun</author>
/// <since>4.0.0</since>
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
    /// <since>4.0.0</since>
    private static readonly JsonSerializerOptions _jsonOptions = new(JsonSerializerDefaults.Web) { WriteIndented = true };

    /// <summary>
    /// Serializes every write, so a conditional clear can compare the file against the revision it expects without a write landing in
    /// between. A half-written file is already ruled out without it, since a write goes to a uniquely named temporary file that is then
    /// moved into place.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>4.0.0</since>
    private readonly SemaphoreSlim _writeLock = new(1, 1);

    /// <summary>
    /// Guards watcher setup and disposal, so setup is attempted at most once and never after the store is disposed. Taken through
    /// <see cref="EnsureWatching"/> on every read, cache hit or not, though once the attempt has run it is held only for a flag test.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>4.0.0</since>
    private readonly Lock _watcherGate = new();

    /// <summary>
    /// The cached state together with the generation it was read under. A read whose generation still matches is answered from here; any
    /// other read goes to the file. It starts empty, and every watcher event and every <see cref="Publish"/> retires it.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>4.0.0</since>
    private volatile CachedState? _cached;

    /// <summary>
    /// The generation the cache is valid for, incremented by every watcher event and every <see cref="Publish"/>. An entry stamped with an
    /// earlier generation is reloaded rather than trusted, which is what stops a read that overlapped a write from publishing the old
    /// value.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>4.0.0</since>
    private long _cacheVersion;

    /// <summary>
    /// The generation a conditional clear last found the file unreadable under. It starts below every generation, so the first such clear
    /// always reads the file.
    /// </summary>
    ///
    /// <remarks>
    /// Set to a generation read before the failed attempt rather than after it, so a watcher event or a <see cref="Publish"/> landing while
    /// that attempt ran leaves the two differing and the next clear reads again.
    /// </remarks>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    private long _unverifiableVersion = -1;

    /// <summary>
    /// Watches the state file so an out-of-band edit is noticed.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>4.0.0</since>
    private FileSystemWatcher? _watcher;

    /// <summary>
    /// Whether watcher setup has already run, or the store has been disposed. Read and written only under <see cref="_watcherGate"/>, so
    /// no second caller can enter setup and no caller can build a watcher after disposal has swept.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>4.0.0</since>
    private bool _watching;

    /// <summary>
    /// Resolves the state file's location under the content root, so the file travels with the deployment rather than the working
    /// directory.
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
    /// Replaces the cache with the state that stands for the file as it is now, retiring every entry read before it. Called after every
    /// write and clear, and after a conditional clear that read something other than the revision it expected, which is what lets the
    /// caller's next read reach that value instead of the one that failed the guard.
    /// </summary>
    ///
    /// <param name="state">
    /// What to serve until the next invalidation: the window just written, the window the file was found to hold, the fail-closed window
    /// standing in for one that does not parse, or <c>null</c> when the file was deleted or is not there.
    /// </param>
    ///
    /// <remarks>
    /// The generation is bumped before the entry is stored, so a read that started earlier declines to store its own result rather than
    /// overwriting this. That read still returns the value it loaded; the reload happens on the next one. If a write or clear that led here
    /// later raises a watcher event, that event costs one reload and cannot resurrect the old value.
    /// </remarks>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>4.0.0</since>
    private void Publish(PersistedMaintenanceState? state) => _cached = new CachedState(Interlocked.Increment(ref _cacheVersion), state);

    /// <summary>
    /// Whether a conditional clear has already found the file unreadable under the generation the cache still stands on, which bounds an
    /// unreadable file to one failed read and its backoffs per generation instead of one per call.
    /// </summary>
    ///
    /// <returns>
    /// <c>true</c> while that attempt still stands, so reading again would open the same file behind the same cache entry;
    /// <c>false</c> once a watcher event or a <see cref="Publish"/> has retired the entry it was made under.
    /// </returns>
    ///
    /// <remarks>
    /// Tested before <see cref="_writeLock"/> is taken, so a caller refused this way neither waits on the lock nor holds it, and again
    /// after it is taken, which catches the callers that were already queued when the failure was recorded.
    /// </remarks>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    private bool IsKnownUnverifiable() => Volatile.Read(ref _unverifiableVersion) == Volatile.Read(ref _cacheVersion);

    /// <summary>
    /// Deletes the state file when the current process can see it, which is what closing a window amounts to on disk.
    /// </summary>
    ///
    /// <exception cref="IOException">The file exists but could not be deleted, so the window it holds stays open.</exception>
    /// <exception cref="UnauthorizedAccessException">The process may not delete the file, so the window it holds stays open.</exception>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>4.0.0</since>
    private void DeleteFile()
    {
        if (File.Exists(FilePath))
            File.Delete(FilePath);
    }

    /// <summary>
    /// Reads and parses the state file, reporting what the read established alongside the value to serve, so a caller that may act only on
    /// the file's own contents can tell those apart from a value that merely stood in for them.
    /// </summary>
    ///
    /// <returns>
    /// <see cref="DiskReadOutcome.Missing"/> with no state when the file is not there; <see cref="DiskReadOutcome.Loaded"/> with the parsed
    /// window; <see cref="DiskReadOutcome.Corrupt"/> with the fail-closed window when the file is there but does not parse; and
    /// <see cref="DiskReadOutcome.Unreadable"/> with the last cached value, which may itself be <c>null</c>, when three attempts to open it
    /// all failed.
    /// </returns>
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

                PersistedMaintenanceState? state = await JsonSerializer.DeserializeAsync<PersistedMaintenanceState>(stream, _jsonOptions);

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
                        FilePath
                    );

                    return new DiskRead(DiskReadOutcome.Unreadable, _cached?.State);
                }

                await Task.Delay(TimeSpan.FromMilliseconds(20 * (attempt + 1)));
            }
        }

        return new DiskRead(DiskReadOutcome.Unreadable, _cached?.State);
    }

    /// <summary>
    /// Builds the fail-closed state used when the file exists but cannot be parsed.
    /// </summary>
    ///
    /// <returns>An enabled state that keeps maintenance active until the file is fixed.</returns>
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
    /// Starts watching the content root for changes to the state file, on the first read. Setup runs once, so when the directory does not
    /// exist at that moment nothing is watched for the life of the store, and unlike a setup that throws, that case is not logged.
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
    /// <since>4.0.0</since>
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
    /// <since>4.0.0</since>
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
    /// <since>4.0.0</since>
    private sealed record CachedState(long Version, PersistedMaintenanceState? State);

    /// <summary>
    /// Pairs the value a read produced with what that read established about the file, so a value the store substituted is never mistaken
    /// for the file's own contents.
    /// </summary>
    ///
    /// <param name="Outcome">What the read established about the file.</param>
    /// <param name="State">The value to serve, which is the file's own contents only for <see cref="DiskReadOutcome.Loaded"/>.</param>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    private sealed record DiskRead(DiskReadOutcome Outcome, PersistedMaintenanceState? State);

    /// <summary>
    /// What a read of the state file established. Only one of these carries a revision the file is known to hold, which is what a
    /// conditional clear has to compare against before it deletes anything.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    private enum DiskReadOutcome
    {
        /// <summary>
        /// The file is not there, so no window is recorded and there is nothing to serve.
        /// </summary>
        ///
        /// <author>Almighty-Shogun</author>
        /// <since>Unreleased</since>
        Missing,

        /// <summary>
        /// The file was opened and parsed, so the accompanying window and its revision are the file's own.
        /// </summary>
        ///
        /// <author>Almighty-Shogun</author>
        /// <since>Unreleased</since>
        Loaded,

        /// <summary>
        /// The file was opened but does not parse, so the accompanying window is the fail-closed one rather than anything the file holds.
        /// </summary>
        ///
        /// <author>Almighty-Shogun</author>
        /// <since>Unreleased</since>
        Corrupt,

        /// <summary>
        /// The file is there but every attempt to open it failed, so the accompanying value is the last cached one and says nothing about
        /// what the file holds now.
        /// </summary>
        ///
        /// <author>Almighty-Shogun</author>
        /// <since>Unreleased</since>
        Unreadable
    }
}
