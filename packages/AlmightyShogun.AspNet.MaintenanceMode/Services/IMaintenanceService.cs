namespace AlmightyShogun.AspNet.MaintenanceMode;

/// <summary>
/// Manages persisted maintenance windows.
/// </summary>
///
/// <author>Almighty-Shogun</author>
/// <since>4.0.0</since>
public interface IMaintenanceService
{
    /// <summary>
    /// Gets the current maintenance state after applying defaults and expiry policy.
    /// </summary>
    ///
    /// <returns>The current maintenance state.</returns>
    ///
    /// <remarks>
    /// An enabled window may be scheduled for a future start and not yet block requests.
    /// </remarks>
    /// 
    /// <author>Almighty-Shogun</author>
    /// <since>4.0.0</since>
    Task<MaintenanceState> GetAsync();

    /// <summary>
    /// Reports whether a maintenance window is enabled.
    /// </summary>
    ///
    /// <returns>
    /// <c>true</c> if a window is enabled, including one scheduled to start later;
    /// otherwise, <c>false</c>.
    /// </returns>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>4.0.0</since>
    Task<bool> IsEnabledAsync();

    /// <summary>
    /// Enables maintenance mode, replacing any existing window.
    /// </summary>
    ///
    /// <param name="request">
    /// The maintenance window to enable. Unset options use configured defaults.
    /// </param>
    ///
    /// <returns>A task that completes when the window has been persisted.</returns>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>4.0.0</since>
    Task EnableAsync(MaintenanceRequest request);

    /// <summary>
    /// Disables maintenance mode and removes the persisted window.
    /// </summary>
    ///
    /// <returns>A task that completes when maintenance mode has been disabled.</returns>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>4.0.0</since>
    Task DisableAsync();
}
