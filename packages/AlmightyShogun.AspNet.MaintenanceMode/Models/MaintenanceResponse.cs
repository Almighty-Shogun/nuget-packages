namespace AlmightyShogun.AspNet.MaintenanceMode;

/// <summary>
/// The response returned by the maintenance endpoint.
/// </summary>
///
/// <author>Almighty-Shogun</author>
/// <since>4.0.0</since>
public sealed record MaintenanceResponse
{
    /// <summary>
    /// The maintenance message.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>4.0.0</since>
    public required string? Message { get; init; }

    /// <summary>
    /// The scheduled start of the window, if any.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>4.0.0</since>
    public required DateTimeOffset? StartsAt { get; init; }

    /// <summary>
    /// The estimated end of the window, if any.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>4.0.0</since>
    public required DateTimeOffset? EndsAt { get; init; }

    /// <summary>
    /// When maintenance mode was enabled.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>4.0.0</since>
    public required DateTimeOffset? EnabledAt { get; init; }
}
