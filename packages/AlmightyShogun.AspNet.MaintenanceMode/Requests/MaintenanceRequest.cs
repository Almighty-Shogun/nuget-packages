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
    /// Gets the explanation a blocked caller is shown. Left unset, <see cref="MaintenanceSettings.DefaultMessage"/> is used, which may
    /// itself be unset, in which case the response carries no message.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    public string? Message { get; init; }

    /// <summary>
    /// Gets when the maintenance window should end. Any offset is accepted, since the comparisons that drive expiry and the
    /// <c>Retry-After</c> header are absolute. Leave it unset for a window with no estimated end.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    public DateTimeOffset? EndsAt { get; init; }

    /// <summary>
    /// Gets when the maintenance window should start. Leave it unset to begin immediately; set it to schedule a window ahead of time, which
    /// the middleware then honors without any external scheduler.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    public DateTimeOffset? StartsAt { get; init; }

    /// <summary>
    /// Gets whether the window lifts itself once <see cref="EndsAt"/> has passed. Left unset, the configured default decides; there is
    /// nothing to lift when no end time is set.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    public bool? AutoDisableWhenExpired { get; init; }

    /// <summary>
    /// Gets whether a blocked request that accepts <c>text/html</c> is redirected to the maintenance path instead of receiving the error
    /// body. A client that does not accept HTML gets the body either way. Left unset, the configured default decides.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    public bool? RedirectBlockedRequests { get; init; }

    /// <summary>
    /// Gets the paths that stay reachable while the window is open, matched in full. A list given here replaces the configured one rather
    /// than adding to it, so an empty list keeps nothing open.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    public IReadOnlyList<string>? AllowedPaths { get; init; }

    /// <summary>
    /// Gets the prefixes that stay reachable while the window is open, matched on segment boundaries, so <c>/api</c> opens
    /// <c>/api/orders</c> but not <c>/apixyz</c>. A list given here replaces the configured one rather than adding to it.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    public IReadOnlyList<string>? AllowedPathPrefixes { get; init; }

    /// <summary>
    /// Gets the IP addresses allowed through while maintenance mode is enabled, so an operator can verify the site before lifting it. A
    /// list given here replaces the configured one rather than adding to it.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    public IReadOnlyList<string>? AllowedIpAddresses { get; init; }
}
