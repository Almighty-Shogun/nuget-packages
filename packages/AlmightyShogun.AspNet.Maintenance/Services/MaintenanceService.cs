using System.Text.Json;
using AlmightyShogun.Utils;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Options;

namespace AlmightyShogun.AspNet.Maintenance;

/// <summary>
/// Stores and reads maintenance mode state from the application content root.
/// </summary>
///
/// <param name="maintenanceOptions">The bound maintenance settings used by the service.</param>
/// <param name="webHostEnvironment">The web host environment used to resolve the content root.</param>
///
/// <author>Almighty-Shogun</author>
/// <since>Unreleased</since>
internal sealed class MaintenanceService(IOptions<MaintenanceSettings> maintenanceOptions, IWebHostEnvironment webHostEnvironment) : IMaintenanceService
{
    /// <summary>
    /// Stores the lock used to serialize reads and writes against the maintenance state file.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    private readonly SemaphoreSlim _fileLock = new(1, 1);

    /// <summary>
    /// Stores the bound maintenance settings used as defaults when writing or reading state.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    private readonly MaintenanceSettings _maintenanceSettings = maintenanceOptions.Value;

    /// <summary>
    /// Gets the absolute path to the maintenance state file in the application content root.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    private string FilePath => Path.Combine(webHostEnvironment.ContentRootPath, "maintenance.json");

    /// <summary>
    /// Stores the JSON serializer options used for the persisted maintenance state file.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    private static readonly JsonSerializerOptions _jsonOptions = new(JsonSerializerDefaults.Web)
    {
        WriteIndented = true
    };

    /// <inheritdoc />
    public async Task<MaintenanceState> GetAsync()
    {
        await _fileLock.WaitAsync();

        try
        {
            MaintenanceState? state = await ReadAsync();

            if (state is null || !state.IsEnabled)
                return CreateDisabledState();

            if (state.EndsAt is null || !state.AutoDisableWhenExpired || state.EndsAt > DateTime.UtcNow)
                return state;

            Clear();

            return CreateDisabledState();
        }
        finally
        {
            _fileLock.Release();
        }
    }

    /// <inheritdoc />
    public async Task<bool> IsEnabledAsync()
    {
        MaintenanceState state = await GetAsync();

        return state.IsEnabled;
    }

    /// <inheritdoc />
    public async Task EnableAsync(MaintenanceRequest request)
    {
        await _fileLock.WaitAsync();

        try
        {
            await WriteAsync(new MaintenanceState
            {
                IsEnabled = true,
                EndsAt = request.EndsAt,
                EnabledAt = DateTime.UtcNow,
                AllowedPaths = ResolveAllowedPaths(request.AllowedPaths),
                Message = request.Message ?? _maintenanceSettings.DefaultMessage,
                AllowedPathPrefixes = ResolveAllowedPathPrefixes(request.AllowedPathPrefixes),
                AutoDisableWhenExpired = request.AutoDisableWhenExpired ?? _maintenanceSettings.AutoDisableWhenExpired,
                RedirectBlockedRequests = request.RedirectBlockedRequests ?? _maintenanceSettings.RedirectBlockedRequests
            });
        }
        finally
        {
            _fileLock.Release();
        }
    }

    /// <inheritdoc />
    public async Task DisableAsync()
    {
        await _fileLock.WaitAsync();

        try
        {
            Clear();
        }
        finally
        {
            _fileLock.Release();
        }
    }

    /// <summary>
    /// Reads the maintenance state file.
    /// </summary>
    ///
    /// <returns>The persisted maintenance state, <c>null</c> when the file does not exist, or a safe enabled state when the file is corrupt or unreadable.</returns>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    private async Task<MaintenanceState?> ReadAsync()
    {
        if (!File.Exists(FilePath))
            return null;

        try
        {
            await using FileStream stream = File.OpenRead(FilePath);

            MaintenanceState? state = await stream.DeserializeAsync<MaintenanceState>(_jsonOptions);

            return state is null ? CreateCorruptState() : NormalizeState(state);
        }
        catch (JsonException)
        {
            return CreateCorruptState();
        }
        catch (NotSupportedException)
        {
            return CreateCorruptState();
        }
        catch (IOException)
        {
            return CreateCorruptState();
        }
        catch (UnauthorizedAccessException)
        {
            return CreateCorruptState();
        }
    }

    /// <summary>
    /// Writes the maintenance state to the state file.
    /// </summary>
    ///
    /// <param name="state">The maintenance state to persist.</param>
    ///
    /// <returns>A task that completes when the state has been written.</returns>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    private async Task WriteAsync(MaintenanceState state)
    {
        string directory = Path.GetDirectoryName(FilePath) ?? webHostEnvironment.ContentRootPath;

        Directory.CreateDirectory(directory);

        string tempFilePath = $"{FilePath}.tmp";

        await using (FileStream stream = File.Create(tempFilePath))
        {
            await state.SerializeAsync(stream, _jsonOptions);
        }

        File.Move(tempFilePath, FilePath, true);
    }

    /// <summary>
    /// Deletes the maintenance state file when it exists.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    private void Clear()
    {
        if (File.Exists(FilePath))
            File.Delete(FilePath);
    }

    /// <summary>
    /// Creates a disabled state from configured defaults.
    /// </summary>
    ///
    /// <returns>A disabled maintenance state populated from configured defaults.</returns>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    private MaintenanceState CreateDisabledState() => new()
    {
        EndsAt = null,
        EnabledAt = null,
        IsEnabled = false,
        AllowedPaths = ResolveAllowedPaths(null),
        Message = _maintenanceSettings.DefaultMessage,
        AllowedPathPrefixes = ResolveAllowedPathPrefixes(null),
        AutoDisableWhenExpired = _maintenanceSettings.AutoDisableWhenExpired,
        RedirectBlockedRequests = _maintenanceSettings.RedirectBlockedRequests
    };

    /// <summary>
    /// Creates an enabled fallback state for corrupt or unreadable state files.
    /// </summary>
    ///
    /// <returns>An enabled maintenance state that keeps maintenance mode active until the file is fixed or disabled.</returns>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    private MaintenanceState CreateCorruptState() => new()
    {
        EndsAt = null,
        IsEnabled = true,
        EnabledAt = DateTime.UtcNow,
        AutoDisableWhenExpired = false,
        AllowedPaths = ResolveAllowedPaths(null),
        AllowedPathPrefixes = ResolveAllowedPathPrefixes(null),
        Message = "Maintenance file is corrupt, please resolve this.",
        RedirectBlockedRequests = _maintenanceSettings.RedirectBlockedRequests
    };

    /// <summary>
    /// Normalizes deserialized maintenance state values that may be missing or null in the persisted JSON file.
    /// </summary>
    ///
    /// <param name="state">The deserialized maintenance state.</param>
    ///
    /// <returns>The maintenance state with non-null path collections.</returns>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    private MaintenanceState NormalizeState(MaintenanceState state) => new()
    {
        EndsAt = state.EndsAt,
        Message = state.Message,
        EnabledAt = state.EnabledAt,
        IsEnabled = state.IsEnabled,
        AllowedPaths = state.AllowedPaths ?? [],
        AutoDisableWhenExpired = state.AutoDisableWhenExpired,
        RedirectBlockedRequests = state.RedirectBlockedRequests,
        AllowedPathPrefixes = state.AllowedPathPrefixes ?? []
    };

    /// <summary>
    /// Resolves exact allowed paths from request values or configured defaults.
    /// </summary>
    ///
    /// <param name="allowedPaths">The request-specific allowed paths.</param>
    ///
    /// <returns>The distinct allowed paths.</returns>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    private string[] ResolveAllowedPaths(IReadOnlyList<string>? allowedPaths)
    {
        IEnumerable<string> paths = allowedPaths ?? _maintenanceSettings.AllowedPaths;

        return paths
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToArray();
    }

    /// <summary>
    /// Resolves allowed path prefixes from request values or configured defaults.
    /// </summary>
    ///
    /// <param name="allowedPathPrefixes">The request-specific allowed path prefixes.</param>
    ///
    /// <returns>The distinct allowed path prefixes.</returns>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    private string[] ResolveAllowedPathPrefixes(IReadOnlyList<string>? allowedPathPrefixes)
        => (allowedPathPrefixes ?? _maintenanceSettings.AllowedPathPrefixes)
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToArray();
}
