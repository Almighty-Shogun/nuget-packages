namespace AlmightyShogun.AspNet.MaintenanceMode;

/// <summary>
/// The window an operator asks for. Every field left unset falls back to the <c>Maintenance</c> configuration section, except the start and
/// end times, which have no configured counterpart and simply stay absent.
/// </summary>
///
/// <author>Almighty-Shogun</author>
/// <since>Unreleased</since>
public sealed record MaintenanceRequest
{
    /// <summary>
    /// The explanation a blocked caller is shown. Left unset, <see cref="IMaintenanceService.EnableAsync"/> decides what the window
    /// carries.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    public string? Message { get; init; }

    /// <summary>
    /// When the maintenance window should end. Leave it unset for a window with no estimated end.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    public DateTimeOffset? EndsAt { get; init; }

    /// <summary>
    /// When the maintenance window should start. Leave it unset to begin immediately; set it to schedule a window ahead of time.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    public DateTimeOffset? StartsAt { get; init; }

    /// <summary>
    /// Whether the window lifts itself once <see cref="EndsAt"/> has passed. Left unset, the configured default decides.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    public bool? AutoDisableWhenExpired { get; init; }

    /// <summary>
    /// Whether a blocked request is redirected to the maintenance path instead of receiving the error body. Left unset, the configured
    /// default decides.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    public bool? RedirectBlockedRequests { get; init; }

    /// <summary>
    /// The paths that stay reachable while the window is open.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    public IReadOnlyList<string>? AllowedPaths { get; init; }

    /// <summary>
    /// The path prefixes that stay reachable while the window is open.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    public IReadOnlyList<string>? AllowedPathPrefixes { get; init; }

    /// <summary>
    /// The IP addresses allowed through while maintenance mode is enabled, so an operator can verify the site before lifting it.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    public IReadOnlyList<string>? AllowedIpAddresses { get; init; }
}
