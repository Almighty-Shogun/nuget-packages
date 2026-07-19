namespace AlmightyShogun.AspNet.Maintenance;

/// <summary>
/// Controls the persisted maintenance mode state.
/// </summary>
///
/// <author>Almighty-Shogun</author>
/// <since>Unreleased</since>
public interface IMaintenanceService
{
    /// <summary>
    /// Gets the current maintenance mode state.
    /// </summary>
    ///
    /// <returns>The current maintenance mode state.</returns>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    Task<MaintenanceState> GetAsync();

    /// <summary>
    /// Checks whether maintenance mode is currently enabled.
    /// </summary>
    ///
    /// <returns><c>true</c> when maintenance mode is enabled; otherwise, <c>false</c>.</returns>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    Task<bool> IsEnabledAsync();

    /// <summary>
    /// Enables maintenance mode using the supplied request values.
    /// </summary>
    ///
    /// <param name="request">The maintenance values to persist.</param>
    ///
    /// <returns>A task representing the asynchronous enable operation.</returns>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    Task EnableAsync(MaintenanceRequest request);

    /// <summary>
    /// Disables maintenance mode and clears the persisted state.
    /// </summary>
    ///
    /// <returns>A task representing the asynchronous disable operation.</returns>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    Task DisableAsync();
}
