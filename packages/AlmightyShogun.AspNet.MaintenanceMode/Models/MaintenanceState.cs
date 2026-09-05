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
/// <since>4.0.0</since>
public sealed record MaintenanceState
{
    /// <summary>
    /// Whether a window is recorded as open.
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
}
