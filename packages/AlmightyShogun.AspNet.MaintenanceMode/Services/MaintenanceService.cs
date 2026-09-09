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
/// <since>4.0.0</since>
internal sealed class MaintenanceService(IOptions<MaintenanceSettings> maintenanceOptions, IMaintenanceStore store) : IMaintenanceService
{
    /// <summary>
    /// The configured defaults, resolved once so every member reads the same settings instance rather than going through
    /// <see cref="IOptions{TOptions}"/> on each call.
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
    /// <returns>
    /// The window as it now stands, never <c>null</c>, with the configured defaults filled in. A synthesized disabled state stands in for a
    /// file recording no open window and for an expired window that was closed; an expired window whose clear could not be verified is
    /// returned rather than replaced by one.
    /// </returns>
    ///
    /// <exception cref="IOException">
    /// An expired window was being closed and its file could not be deleted. The read itself is guarded and does not throw.
    /// </exception>
    /// <exception cref="UnauthorizedAccessException">
    /// The process may not delete the state file of an expired window that was being closed.
    /// </exception>
    /// <exception cref="InvalidOperationException">
    /// The conditional clear reported a value <see cref="MaintenanceClearOutcome"/> does not define, which is rejected here rather than
    /// silently taking one of the three branches that do.
    /// </exception>
    ///
    /// <remarks>
    /// An expired window is closed through <see cref="IMaintenanceStore.TryClearAsync"/> rather than
    /// <see cref="IMaintenanceStore.ClearAsync"/>, so a window opened while this was deciding to expire the old one is not closed with it.
    /// </remarks>
    ///
    /// <remarks>
    /// A <see cref="MaintenanceClearOutcome.Superseded"/> clear sends this round again, and the store has already retired the cache entry
    /// that produced the mismatch, so the next pass reads what the store found rather than comparing the same revision a second time.
    /// </remarks>
    ///
    /// <remarks>
    /// An <see cref="MaintenanceClearOutcome.Unverified"/> clear ends the loop with the expired window rather than another attempt: nothing
    /// was published, so the next pass would carry the same cached window into the same guard. The store established nothing about what the
    /// file holds, so the expired window is returned rather than the disabled state, and it stops expiring for as long as the file stays
    /// unreadable.
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
    /// Fills in the collections a hand-edited file may have omitted.
    /// </summary>
    ///
    /// <param name="state">The window as the file held it, before defaults or the expiry policy are applied.</param>
    ///
    /// <returns>The state with non-null collections.</returns>
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
    /// Builds the open-for-business state returned when no window exists, so a caller never has to distinguish absent from disabled.
    /// </summary>
    ///
    /// <returns>A disabled maintenance state.</returns>
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
    /// Picks the supplied values or the configured defaults, deduplicated.
    /// </summary>
    ///
    /// <param name="supplied">
    /// What the window itself declared, which replaces the configured list rather than adding to it when present.
    /// </param>
    /// <param name="defaults">
    /// The configured list, used only when <paramref name="supplied"/> is <c>null</c>. An empty supplied list is a list, so it suppresses
    /// these rather than falling back to them.
    /// </param>
    ///
    /// <returns>The resolved values, compared case-insensitively when duplicates are removed.</returns>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>4.0.0</since>
    private static IReadOnlyList<string> Resolve(IReadOnlyList<string>? supplied, IReadOnlyList<string> defaults)
        => [.. (supplied ?? defaults).Distinct(StringComparer.OrdinalIgnoreCase)];

    /// <summary>
    /// Picks the supplied paths or the configured defaults, normalized and deduplicated.
    /// </summary>
    ///
    /// <param name="supplied">
    /// What the window itself declared, which replaces the configured list rather than adding to it when present.
    /// </param>
    /// <param name="defaults">
    /// The configured list, used only when <paramref name="supplied"/> is <c>null</c>. An empty supplied list is a list, so it suppresses
    /// these rather than falling back to them.
    /// </param>
    ///
    /// <returns>
    /// The resolved paths, each carrying a leading slash and no trailing one, with the entries that normalize to nothing dropped and
    /// duplicates removed case-insensitively.
    /// </returns>
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
