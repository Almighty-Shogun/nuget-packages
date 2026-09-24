using Microsoft.Extensions.Options;

namespace AlmightyShogun.AspNet.MaintenanceMode;

/// <summary>
/// Manages persisted maintenance state, configured defaults, and expiry.
/// </summary>
///
/// <param name="maintenanceOptions">The configured maintenance settings.</param>
/// <param name="store">The persisted state store.</param>
///
/// <author>Almighty-Shogun</author>
/// <since>4.0.0</since>
internal sealed class MaintenanceService(IOptions<MaintenanceSettings> maintenanceOptions, IMaintenanceStore store) : IMaintenanceService
{
    /// <summary>
    /// The configured maintenance defaults.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>4.0.0</since>
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
        if (request is { StartsAt: { } startsAt, EndsAt: { } endsAt } && endsAt <= startsAt)
            throw new ArgumentException("A maintenance window must end after it starts.", nameof(request));

        await store.WriteAsync(
            new PersistedMaintenanceState
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
    /// Reads the persisted state, applying defaults and conditionally clearing expired windows.
    /// </summary>
    ///
    /// <returns>
    /// The current state, or a disabled state if no window exists or an expired window was cleared.
    /// An expired window is preserved if its clear cannot be verified.
    /// </returns>
    ///
    /// <remarks>
    /// A superseded window is reread so expiry cannot clear a newer window.
    /// </remarks>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>4.0.0</since>
    internal async Task<PersistedMaintenanceState> GetPersistedAsync()
    {
        while (true)
        {
            PersistedMaintenanceState? state = await store.ReadAsync();

            if (state is null || !state.IsEnabled)
                return CreateDisabledState();

            state = ApplyDefaults(state);

            if (state.EndsAt is null || !state.AutoDisableWhenExpired || state.EndsAt > DateTimeOffset.UtcNow)
                return state;

            MaintenanceClearOutcome outcome = await store.TryClearAsync(state.Revision);

            switch (outcome)
            {
                case MaintenanceClearOutcome.Cleared:
                    return CreateDisabledState();

                case MaintenanceClearOutcome.Unverified:
                    return state;

                case MaintenanceClearOutcome.Superseded:
                    continue;

                default:
                    throw new InvalidOperationException($"The conditional clear reported the unknown outcome '{outcome}'.");
            }
        }
    }

    /// <summary>
    /// Applies configured defaults to omitted allow-lists.
    /// </summary>
    ///
    /// <param name="state">The persisted state.</param>
    ///
    /// <returns>The state with resolved allow-lists.</returns>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>4.0.0</since>
    private PersistedMaintenanceState ApplyDefaults(PersistedMaintenanceState state) => state with
    {
        AllowedPaths = ResolvePaths(state.AllowedPaths, _settings.AllowedPaths),
        AllowedIpAddresses = Resolve(state.AllowedIpAddresses, _settings.AllowedIpAddresses),
        AllowedPathPrefixes = ResolvePaths(state.AllowedPathPrefixes, _settings.AllowedPathPrefixes)
    };

    /// <summary>
    /// Creates a disabled state using configured defaults.
    /// </summary>
    ///
    /// <returns>The disabled maintenance state.</returns>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>4.0.0</since>
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
    /// Resolves supplied values or configured defaults, removing case-insensitive duplicates.
    /// </summary>
    ///
    /// <param name="supplied">The supplied values, or null to use defaults.</param>
    /// <param name="defaults">The configured defaults.</param>
    ///
    /// <returns>The resolved values.</returns>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>4.0.0</since>
    private static IReadOnlyList<string> Resolve(IReadOnlyList<string>? supplied, IReadOnlyList<string> defaults)
        => [.. (supplied ?? defaults).Distinct(StringComparer.OrdinalIgnoreCase)];

    /// <summary>
    /// Resolves, normalizes, and deduplicates supplied paths or configured defaults.
    /// </summary>
    ///
    /// <param name="supplied">The supplied paths, or null to use defaults.</param>
    /// <param name="defaults">The configured defaults.</param>
    ///
    /// <returns>The normalized, non-empty paths.</returns>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>4.0.0</since>
    private static IReadOnlyList<string> ResolvePaths(IReadOnlyList<string>? supplied, IReadOnlyList<string> defaults) =>
    [
        .. (supplied ?? defaults).Select(path => MaintenancePath.Normalize(path).Value)
        .Where(path => !string.IsNullOrEmpty(path))
        .Distinct(StringComparer.OrdinalIgnoreCase)!
    ];
}
