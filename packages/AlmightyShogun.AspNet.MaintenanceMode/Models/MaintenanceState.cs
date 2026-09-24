namespace AlmightyShogun.AspNet.MaintenanceMode;

/// <summary>
/// Represents the current maintenance state.
/// </summary>
///
/// <author>Almighty-Shogun</author>
/// <since>4.0.0</since>
public sealed record MaintenanceState
{
    /// <summary>
    /// Whether maintenance mode is enabled, including a window scheduled to start later.
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
}
