namespace AlmightyShogun.AspNet.Maintenance;

/// <summary>
/// Represents values used to enable maintenance mode.
/// </summary>
///
/// <author>Almighty-Shogun</author>
/// <since>Unreleased</since>
public sealed class MaintenanceRequest
{
    /// <summary>
    /// Gets the message to show in maintenance responses.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    public string? Message { get; init; }

    /// <summary>
    /// Gets the UTC time when maintenance mode should end.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    public DateTime? EndsAt { get; init; }

    /// <summary>
    /// Gets whether this request should override automatic disabling when the end time has passed.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    public bool? AutoDisableWhenExpired { get; init; }

    /// <summary>
    /// Gets whether this request should override redirect behavior for blocked requests.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    public bool? RedirectBlockedRequests { get; init; }

    /// <summary>
    /// Gets the exact request paths that should remain available while maintenance mode is enabled.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    public IReadOnlyList<string>? AllowedPaths { get; init; }

    /// <summary>
    /// Gets the request path prefixes that should remain available while maintenance mode is enabled.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    public IReadOnlyList<string>? AllowedPathPrefixes { get; init; }
}
