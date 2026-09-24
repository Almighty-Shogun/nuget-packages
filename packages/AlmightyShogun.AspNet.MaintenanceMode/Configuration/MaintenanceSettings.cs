using System.ComponentModel.DataAnnotations;

namespace AlmightyShogun.AspNet.MaintenanceMode;

/// <summary>
/// Configures the maintenance endpoint and default settings for maintenance windows.
/// </summary>
///
/// <author>Almighty-Shogun</author>
/// <since>4.0.0</since>
public sealed record MaintenanceSettings
{
    /// <summary>
    /// The reserved maintenance endpoint path. Must not contain whitespace,
    /// a query string, or a fragment.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>4.0.0</since>
    [RegularExpression(
        @"^[^\s?#]+$",
        ErrorMessage = "MaintenancePath must be a path such as '/maintenance', with no whitespace, query string, or fragment."
    )]
    public string MaintenancePath { get; init; } = "/maintenance";

    /// <summary>
    /// The default maintenance message.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>4.0.0</since>
    public string? DefaultMessage { get; init; }

    /// <summary>
    /// Whether windows automatically close after their end time by default.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>4.0.0</since>
    public bool AutoDisableWhenExpired { get; init; }

    /// <summary>
    /// Whether blocked requests are redirected to the maintenance endpoint by default.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>4.0.0</since>
    public bool RedirectBlockedRequests { get; init; } = true;

    /// <summary>
    /// Default request paths allowed during maintenance.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>4.0.0</since>
    public IReadOnlyList<string> AllowedPaths { get; init; } = [];

    /// <summary>
    /// Default request path prefixes allowed during maintenance.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>4.0.0</since>
    public IReadOnlyList<string> AllowedPathPrefixes { get; init; } = [];

    /// <summary>
    /// Default IP addresses allowed during maintenance.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>4.0.0</since>
    public IReadOnlyList<string> AllowedIpAddresses { get; init; } = [];
}
