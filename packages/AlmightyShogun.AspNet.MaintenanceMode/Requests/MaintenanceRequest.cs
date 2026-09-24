namespace AlmightyShogun.AspNet.MaintenanceMode;

/// <summary>
/// Defines a maintenance window. Unset options use configured defaults;
/// start and end times remain optional.
/// </summary>
///
/// <author>Almighty-Shogun</author>
/// <since>4.0.0</since>
public sealed record MaintenanceRequest
{
    /// <summary>
    /// The maintenance message.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>4.0.0</since>
    public string? Message { get; init; }

    /// <summary>
    /// The estimated end of the window, if any.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>4.0.0</since>
    public DateTimeOffset? EndsAt { get; init; }

    /// <summary>
    /// The scheduled start of the window, or null to start immediately.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>4.0.0</since>
    public DateTimeOffset? StartsAt { get; init; }

    /// <summary>
    /// Whether maintenance mode automatically disables after the end time.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>4.0.0</since>
    public bool? AutoDisableWhenExpired { get; init; }

    /// <summary>
    /// Whether blocked requests are redirected to the maintenance path.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>4.0.0</since>
    public bool? RedirectBlockedRequests { get; init; }

    /// <summary>
    /// Request paths allowed during maintenance.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>4.0.0</since>
    public IReadOnlyList<string>? AllowedPaths { get; init; }

    /// <summary>
    /// Request path prefixes allowed during maintenance.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>4.0.0</since>
    public IReadOnlyList<string>? AllowedPathPrefixes { get; init; }

    /// <summary>
    /// IP addresses allowed during maintenance.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>4.0.0</since>
    public IReadOnlyList<string>? AllowedIpAddresses { get; init; }
}
