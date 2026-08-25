namespace AlmightyShogun.AspNet.MaintenanceMode;

/// <summary>
/// The current maintenance mode state, as a caller of <c>GetAsync</c> sees it.
/// </summary>
///
/// <remarks>
/// This carries only what a caller needs. The persisted file additionally holds the behavioural settings that were in force when
/// maintenance was enabled, which are configuration rather than state, so the file format can change without changing this type.
/// </remarks>
///
/// <author>Almighty-Shogun</author>
/// <since>Unreleased</since>
public sealed record MaintenanceState
{
    /// <summary>
    /// Gets whether a window is open at all. A scheduled window that has not started yet still reads as enabled here.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    public bool IsEnabled { get; init; }

    /// <summary>
    /// Gets the explanation shown to a blocked caller, falling back to the configured default when the window supplied none.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    public string? Message { get; init; }

    /// <summary>
    /// Gets the scheduled start, absent for a window that began immediately. A window is only active once this has passed.
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
}
