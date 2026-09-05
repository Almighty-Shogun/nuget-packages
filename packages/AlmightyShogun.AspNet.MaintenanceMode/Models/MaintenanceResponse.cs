namespace AlmightyShogun.AspNet.MaintenanceMode;

/// <summary>
/// The body the maintenance path returns. Separate from the internal state so the file's own bookkeeping, such as the allow lists, is never
/// exposed to whoever is being blocked.
/// </summary>
///
/// <remarks>
/// Written straight to the response as JSON under a <c>503</c> status, rather than through the shared error response writer the middleware
/// uses for the body a blocked request receives.
/// </remarks>
///
/// <author>Almighty-Shogun</author>
/// <since>Unreleased</since>
public sealed record MaintenanceResponse
{
    /// <summary>
    /// Gets the explanation recorded on the window itself. A window opened through <see cref="IMaintenanceService.EnableAsync"/> falls back
    /// to the configured default when it supplied none; a hand-edited file that omits it carries no message at all.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    public required string? Message { get; init; }

    /// <summary>
    /// Gets when the window starts, when scheduled ahead.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    public required DateTimeOffset? StartsAt { get; init; }

    /// <summary>
    /// Gets when the window is expected to end.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    public required DateTimeOffset? EndsAt { get; init; }

    /// <summary>
    /// Gets when maintenance mode was enabled.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    public required DateTimeOffset? EnabledAt { get; init; }
}
