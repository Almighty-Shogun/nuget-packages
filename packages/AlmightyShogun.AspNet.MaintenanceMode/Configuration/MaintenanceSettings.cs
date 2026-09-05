using System.ComponentModel.DataAnnotations;

namespace AlmightyShogun.AspNet.MaintenanceMode;

/// <summary>
/// The <c>Maintenance</c> configuration section. Most values are defaults for the windows an operator opens later, so a request that sets
/// the same field wins for that window while the rest still come from here. <see cref="MaintenancePath"/> is the exception: it is fixed for
/// the application and no window can override it.
/// </summary>
///
/// <author>Almighty-Shogun</author>
/// <since>4.0.0</since>
public sealed record MaintenanceSettings
{
    /// <summary>
    /// The path reserved for maintenance mode. The middleware answers this path itself rather than passing it on, so an application
    /// route there is never reached, and a request for it outside a window is answered <c>404</c>.
    /// </summary>
    ///
    /// <remarks>
    /// Validated at startup, so a value carrying whitespace, a query string, or a fragment fails the host rather than producing a path no
    /// request can ever match. An absent value falls back to the default; a value that is only whitespace is treated as malformed rather
    /// than as absent.
    /// </remarks>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>4.0.0</since>
    [RegularExpression(
        @"^[^\s?#]+$",
        ErrorMessage = "MaintenancePath must be a path such as '/maintenance', with no whitespace, query string, or fragment."
    )]
    public string MaintenancePath { get; init; } = "/maintenance";

    /// <summary>
    /// The message shown when the window that was opened supplied none. Left unset, a blocked request carries no explanation at all.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>4.0.0</since>
    public string? DefaultMessage { get; init; }

    /// <summary>
    /// Whether a window lifts itself once its end time passes. Off by default, so a window outlives its estimate rather than reopening
    /// the site while nobody is watching.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>4.0.0</since>
    public bool AutoDisableWhenExpired { get; init; }

    /// <summary>
    /// Whether a blocked request is redirected to the maintenance path instead of receiving the maintenance response directly.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>4.0.0</since>
    public bool RedirectBlockedRequests { get; init; } = true;

    /// <summary>
    /// The paths that stay reachable while a window is open.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>4.0.0</since>
    public IReadOnlyList<string> AllowedPaths { get; init; } = [];

    /// <summary>
    /// The path prefixes that stay reachable while a window is open.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>4.0.0</since>
    public IReadOnlyList<string> AllowedPathPrefixes { get; init; } = [];

    /// <summary>
    /// The IP addresses allowed through while maintenance mode is enabled.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>4.0.0</since>
    public IReadOnlyList<string> AllowedIpAddresses { get; init; } = [];
}
