namespace AlmightyShogun.AspNet.Maintenance;

/// <summary>
/// Represents the persisted maintenance mode state used by the middleware and service.
/// </summary>
///
/// <author>Almighty-Shogun</author>
/// <since>Unreleased</since>
public sealed class MaintenanceState
{
    /// <summary>
    /// Gets whether maintenance mode is currently enabled.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    public bool IsEnabled { get; init; }

    /// <summary>
    /// Gets the message shown in maintenance responses.
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
    /// Gets the UTC time when maintenance mode was enabled.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    public DateTime? EnabledAt { get; init; }

    /// <summary>
    /// Gets whether maintenance mode should disable itself when <see cref="EndsAt"/> has passed.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    public bool AutoDisableWhenExpired { get; init; }

    /// <summary>
    /// Gets whether blocked requests should be redirected to the configured maintenance path.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    public bool RedirectBlockedRequests { get; init; }

    /// <summary>
    /// Gets the exact request paths that remain available while maintenance mode is enabled.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    public IReadOnlyList<string> AllowedPaths { get; init; } = [];

    /// <summary>
    /// Gets the request path prefixes that remain available while maintenance mode is enabled.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    public IReadOnlyList<string> AllowedPathPrefixes { get; init; } = [];
}
