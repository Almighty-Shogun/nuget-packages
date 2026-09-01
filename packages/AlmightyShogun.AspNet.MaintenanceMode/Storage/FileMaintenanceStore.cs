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
    /// Serializes writes so two operators opening a window at once cannot interleave into a half-written file.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    private readonly SemaphoreSlim _writeLock = new(1, 1);

    /// <summary>
    /// The cached state. Every request reads this rather than the disk; without it each one would cost a file read and a deserialize even
    /// with maintenance off. Reads never take <see cref="_writeLock"/>, so they do not contend with each other or with a write.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    private volatile CachedState? _cached;

    /// <summary>
    /// Watches the state file so an out-of-band edit is noticed.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    private FileSystemWatcher? _watcher;

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

        if (_cached is { } cached)
            return cached.State;

        PersistedMaintenanceState? state = await ReadFromDiskAsync();

        _cached = new CachedState(state);

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

            _cached = new CachedState(state);
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
            if (File.Exists(FilePath))
                File.Delete(FilePath);

            _cached = new CachedState(null);
        }
        finally
        {
            _writeLock.Release();
        }
    }

    /// <inheritdoc />
    public void Dispose()
    {
        _watcher?.Dispose();
        _watcher = null;
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
    /// Starts watching the content root for changes to the state file.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    private void EnsureWatching()
    {
        if (_watcher is not null) return;

        string directory = Path.GetDirectoryName(FilePath) ?? webHostEnvironment.ContentRootPath;

        if (!Directory.Exists(directory)) return;

        try
        {
            FileSystemWatcher watcher = new(directory, "maintenance.json")
            {
                EnableRaisingEvents = true,
                NotifyFilter = NotifyFilters.LastWrite | NotifyFilters.FileName | NotifyFilters.CreationTime
            };

            watcher.Changed += OnStateFileChanged;
            watcher.Created += OnStateFileChanged;
            watcher.Deleted += OnStateFileChanged;
            watcher.Renamed += OnStateFileChanged;

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

    /// <summary>
    /// Invalidates the cache after an out-of-band edit, so a file changed by hand takes effect without a restart.
    /// </summary>
    ///
    /// <param name="sender">The watcher that raised the change. Unused: any change to the file invalidates the whole cache.</param>
    /// <param name="eventArgs">The file system event arguments.</param>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    private void OnStateFileChanged(object sender, FileSystemEventArgs eventArgs) => _cached = null;

    /// <summary>
    /// Wraps the cached value, so a cached <c>null</c> is distinguishable from nothing cached.
    /// </summary>
    ///
    /// <param name="State">The cached state.</param>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    private sealed record CachedState(PersistedMaintenanceState? State);
}
