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
    /// The explanation recorded on the window, absent when it carries none.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    public required string? Message { get; init; }

    /// <summary>
    /// When the window starts, when scheduled ahead.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    public required DateTimeOffset? StartsAt { get; init; }

    /// <summary>
    /// When the window is expected to end.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    public required DateTimeOffset? EndsAt { get; init; }

    /// <summary>
    /// When maintenance mode was enabled.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    public required DateTimeOffset? EnabledAt { get; init; }
}
