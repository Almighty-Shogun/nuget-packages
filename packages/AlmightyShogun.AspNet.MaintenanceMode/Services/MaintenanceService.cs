using Microsoft.Extensions.Options;

namespace AlmightyShogun.AspNet.MaintenanceMode;

/// <summary>
/// Applies expiry policy and configured defaults over the persisted maintenance state.
/// </summary>
///
/// <param name="maintenanceOptions">The bound maintenance settings used as defaults.</param>
/// <param name="store">The store that owns reading and writing the state.</param>
///
/// <author>Almighty-Shogun</author>
/// <since>Unreleased</since>
internal sealed class MaintenanceService(IOptions<MaintenanceSettings> maintenanceOptions, IMaintenanceStore store) : IMaintenanceService
{
    /// <summary>
    /// The configured defaults, read once at construction, so a window opened mid-request cannot see a half-changed configuration.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    private readonly MaintenanceSettings _settings = maintenanceOptions.Value;

    /// <inheritdoc />
    public async Task<MaintenanceState> GetAsync()
    {
        PersistedMaintenanceState state = await GetPersistedAsync();

        return state.ToState();
    }

    /// <inheritdoc />
    public async Task<bool> IsEnabledAsync()
    {
        PersistedMaintenanceState state = await GetPersistedAsync();

        return state.IsEnabled;
    }

    /// <inheritdoc />
    public async Task EnableAsync(MaintenanceRequest request)
    {
        if (request.StartsAt is { } startsAt && request.EndsAt is { } endsAt && endsAt <= startsAt)
            throw new ArgumentException("A maintenance window must end after it starts.", nameof(request));

        await store.WriteAsync(new PersistedMaintenanceState
        {
            Revision = Guid.NewGuid(),
            IsEnabled = true,
            StartsAt = request.StartsAt,
            EndsAt = request.EndsAt,
            EnabledAt = DateTimeOffset.UtcNow,
            Message = request.Message ?? _settings.DefaultMessage,
            AllowedPaths = ResolvePaths(request.AllowedPaths, _settings.AllowedPaths),
            AllowedPathPrefixes = ResolvePaths(request.AllowedPathPrefixes, _settings.AllowedPathPrefixes),
            AllowedIpAddresses = Resolve(request.AllowedIpAddresses, _settings.AllowedIpAddresses),
            AutoDisableWhenExpired = request.AutoDisableWhenExpired ?? _settings.AutoDisableWhenExpired,
            RedirectBlockedRequests = request.RedirectBlockedRequests ?? _settings.RedirectBlockedRequests
        });
    }

    /// <inheritdoc />
    public Task DisableAsync() => store.ClearAsync();

    /// <summary>
    /// Reads the persisted state, applying defaults and the expiry policy.
    /// </summary>
    ///
    /// <returns>The effective persisted state.</returns>
    ///
    /// <remarks>
    /// An expired window is closed through <see cref="IMaintenanceStore.TryClearAsync"/> rather than
    /// <see cref="IMaintenanceStore.ClearAsync"/>, so a window opened while this was deciding to expire the old one is not closed with it.
    /// When the revision no longer matches, the read is repeated against whatever is recorded now.
    /// </remarks>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    internal async Task<PersistedMaintenanceState> GetPersistedAsync()
    {
        PersistedMaintenanceState? state = await store.ReadAsync();

        if (state is null || !state.IsEnabled)
            return CreateDisabledState();

        state = ApplyDefaults(state);

        if (state.EndsAt is null || !state.AutoDisableWhenExpired || state.EndsAt > DateTimeOffset.UtcNow)
            return state;

        return await store.TryClearAsync(state.Revision) ? CreateDisabledState() : await GetPersistedAsync();
    }

    /// <summary>
    /// Fills in the collections a hand-edited file may have omitted.
    /// </summary>
    ///
    /// <param name="state">The window as the file held it, before defaults or the expiry policy are applied.</param>
    ///
    /// <returns>The state with non-null collections.</returns>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    private PersistedMaintenanceState ApplyDefaults(PersistedMaintenanceState state) => state with
    {
        AllowedPaths = ResolvePaths(state.AllowedPaths, _settings.AllowedPaths),
        AllowedIpAddresses = Resolve(state.AllowedIpAddresses, _settings.AllowedIpAddresses),
        AllowedPathPrefixes = ResolvePaths(state.AllowedPathPrefixes, _settings.AllowedPathPrefixes)
    };

    /// <summary>
    /// Builds the open-for-business state returned when no window exists, so a caller never has to distinguish absent from disabled.
    /// </summary>
    ///
    /// <returns>A disabled maintenance state.</returns>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    private PersistedMaintenanceState CreateDisabledState() => new()
    {
        IsEnabled = false,
        Message = _settings.DefaultMessage,
        AutoDisableWhenExpired = _settings.AutoDisableWhenExpired,
        RedirectBlockedRequests = _settings.RedirectBlockedRequests,
        AllowedPaths = ResolvePaths(null, _settings.AllowedPaths),
        AllowedIpAddresses = Resolve(null, _settings.AllowedIpAddresses),
        AllowedPathPrefixes = ResolvePaths(null, _settings.AllowedPathPrefixes),
    };

    /// <summary>
    /// Picks the supplied values or the configured defaults, deduplicated.
    /// </summary>
    ///
    /// <param name="supplied">
    /// What the window itself declared, which replaces the configured list rather than adding to it when present.
    /// </param>
    /// <param name="defaults">The configured list, used only when the window declared none of its own.</param>
    ///
    /// <returns>The resolved values.</returns>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    private static IReadOnlyList<string> Resolve(IReadOnlyList<string>? supplied, IReadOnlyList<string> defaults)
        => [.. (supplied ?? defaults).Distinct(StringComparer.OrdinalIgnoreCase)];

    /// <summary>
    /// Picks the supplied paths or the configured defaults, normalized and deduplicated.
    /// </summary>
    ///
    /// <param name="supplied">
    /// What the window itself declared, which replaces the configured list rather than adding to it when present.
    /// </param>
    /// <param name="defaults">The configured list, used only when the window declared none of its own.</param>
    ///
    /// <returns>The resolved paths, normalized once so the request path does not have to be.</returns>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    private static IReadOnlyList<string> ResolvePaths(IReadOnlyList<string>? supplied, IReadOnlyList<string> defaults) =>
    [
        .. (supplied ?? defaults).Select(path => MaintenancePath.Normalize(path).Value)
        .Where(path => !string.IsNullOrEmpty(path))
        .Distinct(StringComparer.OrdinalIgnoreCase)!
    ];
}
