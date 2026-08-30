namespace AlmightyShogun.AspNet.MaintenanceMode;

/// <summary>
/// The on-disk shape of the state file. Kept separate from the public model so the file format can change without changing what callers
/// see, and so a hand-edited file with missing fields still deserializes.
/// </summary>
///
/// <remarks>
/// Internal and separate from <see cref="MaintenanceState"/> , so the file can carry the behavioral settings that were in force when
/// maintenance was enabled without those appearing on the public model. Collections are nullable here because a hand-edited file may omit
/// them; defaults are applied on read.
/// </remarks>
///
/// <author>Almighty-Shogun</author>
/// <since>Unreleased</since>
internal sealed record PersistedMaintenanceState
{
    /// <summary>
    /// Gets whether a window is recorded as open, which a hand-edited file can set directly.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    public bool IsEnabled { get; init; }

    /// <summary>
    /// Gets the explanation shown to a blocked caller. A window opened through the service falls back to the configured default when it
    /// supplied none; a hand-edited file omitting this keeps no message, since only the collections are filled in on read.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    public string? Message { get; init; }

    /// <summary>
    /// Gets the scheduled start, absent for a window that began immediately.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    public DateTimeOffset? StartsAt { get; init; }

    /// <summary>
    /// Gets the estimated end of the window, which drives the <c>Retry-After</c> header and the automatic lift when that is enabled.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    public DateTimeOffset? EndsAt { get; init; }

    /// <summary>
    /// Gets when the window was opened, which is what tells an operator how long the site has been down.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    public DateTimeOffset? EnabledAt { get; init; }

    /// <summary>
    /// Gets whether maintenance mode disables itself once the end time has passed.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    public bool AutoDisableWhenExpired { get; init; }

    /// <summary>
    /// Gets whether blocked requests are redirected to the maintenance path.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    public bool RedirectBlockedRequests { get; init; }

    /// <summary>
    /// Gets the exact paths this window keeps open, which replaced rather than extended the configured list.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    public IReadOnlyList<string>? AllowedPaths { get; init; }

    /// <summary>
    /// Gets the prefixes this window keeps open, matched on segment boundaries as the configured ones are.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    public IReadOnlyList<string>? AllowedPathPrefixes { get; init; }

    /// <summary>
    /// Gets the IP addresses allowed through while maintenance mode is enabled.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    public IReadOnlyList<string>? AllowedIpAddresses { get; init; }

    /// <summary>
    /// Converts the stored window into what a caller sees, dropping the fields that exist only for the file's own bookkeeping.
    /// </summary>
    ///
    /// <returns>The public maintenance state.</returns>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    internal MaintenanceState ToState() => new()
    {
        Message = Message,
        EndsAt = EndsAt,
        StartsAt = StartsAt,
        EnabledAt = EnabledAt,
        IsEnabled = IsEnabled
    };
}
