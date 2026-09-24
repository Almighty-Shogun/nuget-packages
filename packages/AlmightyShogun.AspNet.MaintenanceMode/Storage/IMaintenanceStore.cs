namespace AlmightyShogun.AspNet.MaintenanceMode;

/// <summary>
/// Reads, writes, and clears persisted maintenance state.
/// </summary>
///
/// <author>Almighty-Shogun</author>
/// <since>4.0.0</since>
internal interface IMaintenanceStore
{
    /// <summary>
    /// Reads the current maintenance state, using the cache when valid.
    /// </summary>
    ///
    /// <returns>
    /// The current state, or <c>null</c> when no state is available.
    /// A corrupt file produces a fail-closed state; an unreadable file may
    /// produce the last cached state or <c>null</c>.
    /// </returns>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>4.0.0</since>
    ValueTask<PersistedMaintenanceState?> ReadAsync();

    /// <summary>
    /// Replaces the persisted maintenance state.
    /// </summary>
    ///
    /// <param name="state">The state to persist.</param>
    ///
    /// <returns>A task that completes when the state has been written.</returns>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>4.0.0</since>
    Task WriteAsync(PersistedMaintenanceState state);

    /// <summary>
    /// Unconditionally removes the persisted maintenance state.
    /// </summary>
    ///
    /// <returns>A task that completes when the state has been cleared.</returns>
    ///
    /// <remarks>
    /// Use <see cref="TryClearAsync"/> when a newer maintenance window must be preserved.
    /// </remarks>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>4.0.0</since>
    Task ClearAsync();

    /// <summary>
    /// Clears the persisted state only if its revision still matches.
    /// </summary>
    ///
    /// <param name="expectedRevision">The revision of the window to clear.</param>
    ///
    /// <returns>
    /// <see cref="MaintenanceClearOutcome.Cleared"/> if the matching window was removed;
    /// <see cref="MaintenanceClearOutcome.Superseded"/> if the persisted state no longer matches;
    /// or <see cref="MaintenanceClearOutcome.Unverified"/> if the file could not be verified
    /// and was left unchanged.
    /// </returns>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>4.0.0</since>
    Task<MaintenanceClearOutcome> TryClearAsync(Guid expectedRevision);
}
