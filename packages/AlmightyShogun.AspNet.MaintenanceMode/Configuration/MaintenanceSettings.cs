namespace AlmightyShogun.AspNet.MaintenanceMode;

/// <summary>
/// The <c>Maintenance</c> configuration section. Most values are defaults for the windows an operator opens later, so a request that sets
/// the same field wins for that window while the rest still come from here. <see cref="MaintenancePath"/> is the exception: it is fixed for
/// the application and no window can override it.
/// </summary>
///
/// <author>Almighty-Shogun</author>
/// <since>Unreleased</since>
public sealed record MaintenanceSettings
{
    /// <summary>
    /// Gets the path that answers with the maintenance details rather than being blocked, so a blocked visitor has somewhere to be sent and
    /// a status page has something to poll.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    public string MaintenancePath { get; init; } = "/maintenance";

    /// <summary>
    /// Gets the message shown when the window that was opened supplied none. Left unset, a blocked request carries no explanation at all.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    public string? DefaultMessage { get; init; }

    /// <summary>
    /// Gets whether a window lifts itself once its end time passes. Off by default, so a window outlives its estimate rather than reopening
    /// the site while nobody is watching.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    public bool AutoDisableWhenExpired { get; init; }

    /// <summary>
    /// Gets whether a blocked request is redirected to the maintenance path instead of receiving the maintenance response directly. Only a
    /// client that accepts <c>text/html</c> is redirected; anything else gets the response body whatever this says.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    public bool RedirectBlockedRequests { get; init; } = true;

    /// <summary>
    /// Gets the paths that stay reachable, matched exactly. A window that names its own list replaces this rather than adding to it.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    public IReadOnlyList<string> AllowedPaths { get; init; } = [];

    /// <summary>
    /// Gets the prefixes that stay reachable. Matching respects segment boundaries, so <c>/api</c> opens <c>/api/orders</c> but not
    /// <c>/apixyz</c> .
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    public IReadOnlyList<string> AllowedPathPrefixes { get; init; } = [];

    /// <summary>
    /// Gets the IP addresses allowed through while maintenance mode is enabled, used when a request does not supply its own list.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    public IReadOnlyList<string> AllowedIpAddresses { get; init; } = [];
}
