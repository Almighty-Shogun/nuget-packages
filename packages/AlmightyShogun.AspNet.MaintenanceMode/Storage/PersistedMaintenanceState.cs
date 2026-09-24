namespace AlmightyShogun.AspNet.MaintenanceMode;

/// <summary>
/// Represents the persisted maintenance window and its request-enforcement settings.
/// </summary>
///
/// <author>Almighty-Shogun</author>
/// <since>4.0.0</since>
internal sealed record PersistedMaintenanceState
{
    /// <summary>
    /// Identifies the window for revision-checked clearing.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>4.0.0</since>
    public Guid Revision { get; init; }

    /// <summary>
    /// Whether maintenance mode is enabled.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>4.0.0</since>
    public bool IsEnabled { get; init; }

    /// <summary>
    /// The maintenance message.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>4.0.0</since>
    public string? Message { get; init; }

    /// <summary>
    /// The scheduled start of the window, if any.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>4.0.0</since>
    public DateTimeOffset? StartsAt { get; init; }

    /// <summary>
    /// The estimated end of the window, if any.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>4.0.0</since>
    public DateTimeOffset? EndsAt { get; init; }

    /// <summary>
    /// When maintenance mode was enabled.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>4.0.0</since>
    public DateTimeOffset? EnabledAt { get; init; }

    /// <summary>
    /// Whether maintenance mode automatically disables after the end time.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>4.0.0</since>
    public bool AutoDisableWhenExpired { get; init; }

    /// <summary>
    /// Whether blocked requests are redirected to the maintenance path.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>4.0.0</since>
    public bool RedirectBlockedRequests { get; init; }

    /// <summary>
    /// Allowed request paths. Null uses configured defaults.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>4.0.0</since>
    public IReadOnlyList<string>? AllowedPaths { get; init; }

    /// <summary>
    /// Allowed request path prefixes. Null uses configured defaults.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>4.0.0</since>
    public IReadOnlyList<string>? AllowedPathPrefixes { get; init; }

    /// <summary>
    /// Allowed IP addresses. Null uses configured defaults.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>4.0.0</since>
    public IReadOnlyList<string>? AllowedIpAddresses { get; init; }

    /// <summary>
    /// Converts the persisted window to its public state.s
    /// </summary>
    ///
    /// <returns>The public maintenance state.</returns>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>4.0.0</since>
    internal MaintenanceState ToState() => new()
    {
        Message = Message,
        EndsAt = EndsAt,
        StartsAt = StartsAt,
        EnabledAt = EnabledAt,
        IsEnabled = IsEnabled
    };
}
