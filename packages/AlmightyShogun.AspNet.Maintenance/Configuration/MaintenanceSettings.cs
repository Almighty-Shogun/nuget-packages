namespace AlmightyShogun.AspNet.Maintenance;

/// <summary>
/// Represents the maintenance mode configuration used by ASP.NET Maintenance.
/// </summary>
///
/// <author>Almighty-Shogun</author>
/// <since>Unreleased</since>
public sealed class MaintenanceSettings
{
    /// <summary>
    /// Gets or sets the request path that returns maintenance details while maintenance mode is enabled.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    public string MaintenancePath { get; set; } = "/maintenance";

    /// <summary>
    /// Gets or sets the fallback message used when a maintenance request does not provide its own message.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    public string? DefaultMessage { get; set; }

    /// <summary>
    /// Gets or sets whether maintenance mode should disable itself when the configured end time has passed.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    public bool AutoDisableWhenExpired { get; set; } = false;

    /// <summary>
    /// Gets or sets whether blocked requests should be redirected to the maintenance path instead of receiving the maintenance response directly.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    public bool RedirectBlockedRequests { get; set; } = true;

    /// <summary>
    /// Gets or sets the exact request paths that should remain available while maintenance mode is enabled.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    public List<string> AllowedPaths { get; set; } = [];

    /// <summary>
    /// Gets or sets the request path prefixes that should remain available while maintenance mode is enabled.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    public List<string> AllowedPathPrefixes { get; set; } = [];
}
