namespace AlmightyShogun.AspNet.MaintenanceMode;

/// <summary>
/// The on-disk shape of the state file. Kept separate from the public model so the file format can change without changing what callers
/// see, and so a hand-edited file with missing fields still deserializes.
/// </summary>
///
/// <remarks>
/// Internal and separate from <see cref="MaintenanceState"/>, so the file can carry the behavioral settings that were in force when
/// maintenance was enabled without those appearing on the public model. Collections are nullable here because a hand-edited file may omit
/// them; defaults are applied on read.
/// </remarks>
///
/// <author>Almighty-Shogun</author>
/// <since>4.0.0</since>
internal sealed record PersistedMaintenanceState
{
    /// <summary>
    /// The identity of this particular window, issued fresh each time one is opened. Compared by
    /// <see cref="IMaintenanceStore.TryClearAsync"/> so expiring a window cannot close a different one written since it was read.
    /// </summary>
    ///
    /// <remarks>
    /// A hand-edited file that omits it deserializes as <see cref="Guid.Empty"/>, which still compares equal to itself.
    /// </remarks>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>4.0.0</since>
    public Guid Revision { get; init; }

    /// <summary>
    /// Whether a window is recorded as open, which a hand-edited file can set directly.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>4.0.0</since>
    public bool IsEnabled { get; init; }

    /// <summary>
    /// The explanation recorded on the window, absent when it carries none.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>4.0.0</since>
    public string? Message { get; init; }

    /// <summary>
    /// The scheduled start, absent for a window that began immediately.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>4.0.0</since>
    public DateTimeOffset? StartsAt { get; init; }

    /// <summary>
    /// The estimated end of the window, absent for one opened with no estimate.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>4.0.0</since>
    public DateTimeOffset? EndsAt { get; init; }

    /// <summary>
    /// When the window was opened, which is what tells an operator how long the site has been down.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>4.0.0</since>
    public DateTimeOffset? EnabledAt { get; init; }

    /// <summary>
    /// Whether maintenance mode disables itself once the end time has passed.
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
    /// The paths this window keeps open.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>4.0.0</since>
    public IReadOnlyList<string>? AllowedPaths { get; init; }

    /// <summary>
    /// The path prefixes this window keeps open.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>4.0.0</since>
    public IReadOnlyList<string>? AllowedPathPrefixes { get; init; }

    /// <summary>
    /// The IP addresses allowed through while maintenance mode is enabled.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>4.0.0</since>
    public IReadOnlyList<string>? AllowedIpAddresses { get; init; }

    /// <summary>
    /// Converts the stored window into what a caller sees, dropping the fields that exist only for the file's own bookkeeping.
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
