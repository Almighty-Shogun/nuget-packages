namespace AlmightyShogun.Hangfire.RecurringJobs;

/// <summary>
/// Overrides settings declared by a recurring job.
/// </summary>
///
/// <author>Almighty-Shogun</author>
/// <since>4.0.0</since>
public sealed record RecurringJobOverride
{
    /// <summary>
    /// Whether the job is scheduled.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>4.0.0</since>
    public bool? Enabled { get; init; }

    /// <summary>
    /// The cron expression to use instead of the declared one.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>4.0.0</since>
    public string? CronExpression { get; init; }

    /// <summary>
    /// The time zone to use instead of the declared one.
    /// </summary>
    ///
    /// <remarks>
    /// When unset, the declared time zone is preserved. Use <see cref="ClearTimeZone"/> to explicitly clear it.
    /// </remarks>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>4.0.0</since>
    public string? TimeZone { get; init; }

    /// <summary>
    /// Whether the declared time zone should be cleared.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    public bool ClearTimeZone { get; init; }

    /// <summary>
    /// The queue to use instead of the declared one.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>4.0.0</since>
    public string? Queue { get; init; }

    /// <summary>
    /// Whether the declared queue should be cleared.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    public bool ClearQueue { get; init; }
}
