namespace AlmightyShogun.AspNet.MaintenanceMode;

/// <summary>
/// Reads and writes the window that survives a restart. Kept behind an interface so the file-backed default can be swapped for shared
/// storage when more than one instance serves the application.
/// </summary>
///
/// <author>Almighty-Shogun</author>
/// <since>Unreleased</since>
internal interface IMaintenanceStore
{
    /// <summary>
    /// Gets the current persisted state, from memory when it has already been read.
    /// </summary>
    ///
    /// <returns>The persisted state, or <c>null</c> when no window is recorded, whether never opened or since closed.</returns>
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
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    Task WriteAsync(PersistedMaintenanceState state);

    /// <summary>
    /// Deletes the window and clears the cache, which is what closing maintenance mode amounts to.
    /// </summary>
    ///
    /// <returns>A task that completes when the state has been removed.</returns>
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
    /// Deletes the window only while it is still the one the caller read, so expiring a window cannot close a newer one opened in the
    /// meantime.
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
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    Task<bool> TryClearAsync(Guid expectedRevision);
}
