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
    /// <returns>The persisted state, or <c>null</c> when maintenance mode has never been enabled.</returns>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    ValueTask<PersistedMaintenanceState?> ReadAsync();

    /// <summary>
    /// Writes the window and refreshes the cache in one step, so a read straight after a write cannot see the old value.
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
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    Task ClearAsync();
}
