namespace AlmightyShogun.AspNet.MaintenanceMode;

/// <summary>
/// Reads and writes the window that survives a restart. Internal: <see cref="FileMaintenanceStore"/> is the only implementation, and
/// <c>AddMaintenanceMode</c> registers it directly.
/// </summary>
///
/// <author>Almighty-Shogun</author>
/// <since>Unreleased</since>
internal interface IMaintenanceStore
{
    /// <summary>
    /// Gets the current persisted state, which may come from a cache rather than the backing store. A write through this store retires
    /// that cache, as does an out-of-band change the store notices.
    /// </summary>
    ///
    /// <returns>
    /// The persisted state, or <c>null</c> when no window is recorded, whether never opened or since closed. Also <c>null</c>
    /// when the file exists but every read attempt failed and nothing has been cached yet, which a caller cannot tell apart
    /// from no window at all.
    /// </returns>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    ValueTask<PersistedMaintenanceState?> ReadAsync();

    /// <summary>
    /// Writes the window and refreshes the cache in one step, so a read that begins after this returns cannot see the old value.
    /// </summary>
    ///
    /// <param name="state">The window to write, replacing whatever the file held rather than merging with it.</param>
    ///
    /// <returns>A task that completes when the state has been written.</returns>
    ///
    /// <exception cref="IOException">
    /// The directory could not be created, or the state file could not be written or moved into place. Nothing here catches it, so the
    /// cache is left holding whatever it held before and the file keeps the previous window.
    /// </exception>
    /// <exception cref="UnauthorizedAccessException">The process may not write the state file.</exception>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    Task WriteAsync(PersistedMaintenanceState state);

    /// <summary>
    /// Deletes the window and clears the cache, which is what closing maintenance mode amounts to.
    /// </summary>
    ///
    /// <returns>A task that completes when the state has been removed.</returns>
    ///
    /// <exception cref="IOException">The state file exists but could not be deleted, leaving the window open.</exception>
    /// <exception cref="UnauthorizedAccessException">The process may not delete the state file, leaving the window open.</exception>
    ///
    /// <remarks>
    /// Unconditional: it closes whatever window is recorded, including one written since the caller last read. Use
    /// <see cref="TryClearAsync"/> to close only the window the caller actually saw.
    /// </remarks>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    Task ClearAsync();

    /// <summary>
    /// Deletes the window only while it is still the one the caller read, so expiring a window does not close a newer one
    /// opened through the service in the meantime. Two hand-edited windows that both omit a revision compare equal, so the
    /// guard does not separate them.
    /// </summary>
    ///
    /// <param name="expectedRevision">
    /// The <see cref="PersistedMaintenanceState.Revision"/> the caller acted on. The file is re-read under the write lock and compared
    /// against this before anything is deleted.
    /// </param>
    ///
    /// <returns>
    /// <c>true</c> when the window matched and was closed; <c>false</c> when it had already been replaced or closed, leaving whatever is
    /// recorded now untouched. A caller that gets <c>false</c> should read again rather than assume maintenance is off.
    /// </returns>
    ///
    /// <exception cref="IOException">
    /// The revision matched but the state file could not be deleted, leaving the window open. The read that precedes the delete is
    /// guarded and does not throw.
    /// </exception>
    /// <exception cref="UnauthorizedAccessException">
    /// The revision matched but the process may not delete the state file, leaving the window open.
    /// </exception>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    Task<bool> TryClearAsync(Guid expectedRevision);
}
