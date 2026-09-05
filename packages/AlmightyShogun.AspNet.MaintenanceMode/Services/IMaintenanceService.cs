namespace AlmightyShogun.AspNet.MaintenanceMode;

/// <summary>
/// Opens, closes, and reads the maintenance window. The state outlives the process because it is persisted, so a restart during a window
/// does not reopen the site.
/// </summary>
///
/// <author>Almighty-Shogun</author>
/// <since>Unreleased</since>
public interface IMaintenanceService
{
    /// <summary>
    /// Reads the current window with the configured defaults and the expiry policy already applied. It does not report what the middleware
    /// would do with a request: the allow lists are left off <see cref="MaintenanceState"/> entirely, and its
    /// <see cref="MaintenanceState.IsEnabled"/> is the recorded flag rather than whether the window has started.
    /// </summary>
    ///
    /// <returns>The current maintenance mode state.</returns>
    ///
    /// <exception cref="IOException">
    /// An expired window that lifts itself was being closed and its file could not be deleted. The read itself is guarded and does not
    /// throw.
    /// </exception>
    /// <exception cref="UnauthorizedAccessException">
    /// The process may not delete the state file of an expired window that was being closed.
    /// </exception>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    Task<MaintenanceState> GetAsync();

    /// <summary>
    /// Reports whether a window is recorded as open, which is broader than requests being blocked: a window scheduled for later reads as
    /// enabled here while the middleware still serves everything.
    /// </summary>
    ///
    /// <returns><c>true</c> when maintenance mode is enabled; otherwise, <c>false</c>.</returns>
    ///
    /// <exception cref="IOException">
    /// An expired window that lifts itself was being closed and its file could not be deleted. The read itself is guarded and does not
    /// throw.
    /// </exception>
    /// <exception cref="UnauthorizedAccessException">
    /// The process may not delete the state file of an expired window that was being closed.
    /// </exception>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    Task<bool> IsEnabledAsync();

    /// <summary>
    /// Opens a window, taking any field the request leaves unset from configuration. Called again, it replaces the window rather than
    /// merging.
    /// </summary>
    ///
    /// <param name="request">
    /// The window to open. A field it leaves unset is taken from configuration where there is a configured counterpart; the start and end
    /// times have none and stay absent.
    /// </param>
    ///
    /// <returns>A task representing the asynchronous enable operation.</returns>
    ///
    /// <exception cref="ArgumentException">
    /// The request ends at, or before it starts, which describes a window that can never be open. A window with only one of the two times
    /// set is accepted: an open-ended window and one that has already begun are both meaningful.
    /// </exception>
    /// <exception cref="IOException">The state file could not be written, so no window is opened.</exception>
    /// <exception cref="UnauthorizedAccessException">The process may not write the state file, so no window is opened.</exception>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    Task EnableAsync(MaintenanceRequest request);

    /// <summary>
    /// Closes the window and removes the persisted file, so a restart afterward comes back with the site open.
    /// </summary>
    ///
    /// <returns>A task representing the asynchronous disable operation.</returns>
    ///
    /// <exception cref="IOException">The state file exists but could not be deleted, so the window stays open.</exception>
    /// <exception cref="UnauthorizedAccessException">The process may not delete the state file, so the window stays open.</exception>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    Task DisableAsync();
}
