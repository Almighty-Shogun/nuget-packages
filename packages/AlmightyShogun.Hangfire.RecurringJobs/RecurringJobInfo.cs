namespace AlmightyShogun.Hangfire.RecurringJobs;

/// <summary>
/// Describes a discovered recurring job after configuration overrides are applied.
/// </summary>
///
/// <author>Almighty-Shogun</author>
/// <since>4.0.0</since>
public sealed record RecurringJobInfo
{
    /// <summary>
    /// The recurring job id.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>4.0.0</since>
    public required string JobId { get; init; }

    /// <summary>
    /// The five- or six-field cron expression the job is scheduled with.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>4.0.0</since>
    public required string CronExpression { get; init; }

    /// <summary>
    /// The concrete type implementing the recurring job.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>4.0.0</since>
    public required Type JobType { get; init; }

    /// <summary>
    ///  The time zone the cron expression is evaluated in, or <c>null</c> for UTC.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>4.0.0</since>
    public required string? TimeZone { get; init; }

    /// <summary>
    /// The queue the job is enqueued on, or <c>null</c> for the Hangfire default.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>4.0.0</since>
    public required string? Queue { get; init; }
}
