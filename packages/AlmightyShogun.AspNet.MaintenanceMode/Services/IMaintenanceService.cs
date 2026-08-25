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
    /// Reads the current window with the configured defaults and the expiry policy already applied, so a caller sees what the middleware
    /// sees.
    /// </summary>
    ///
    /// <returns>The current maintenance mode state.</returns>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    Task<MaintenanceState> GetAsync();

    /// <summary>
    /// Reports whether requests are being blocked right now, which is narrower than a window existing: a scheduled one is not yet in force.
    /// </summary>
    ///
    /// <returns><c>true</c> when maintenance mode is enabled; otherwise, <c>false</c>.</returns>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    Task<bool> IsEnabledAsync();

    /// <summary>
    /// Opens a window, taking any field the request leaves unset from configuration. Called again, it replaces the window rather than
    /// merging.
    /// </summary>
    ///
    /// <param name="request">The window to open. Any field it leaves unset is taken from configuration rather than left empty.</param>
    ///
    /// <returns>A task representing the asynchronous enable operation.</returns>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    Task EnableAsync(MaintenanceRequest request);

    /// <summary>
    /// Closes the window and removes the persisted file, so a restart afterwards comes back with the site open.
    /// </summary>
    ///
    /// <returns>A task representing the asynchronous disable operation.</returns>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    Task DisableAsync();
}
