namespace AlmightyShogun.Hangfire.RecurringJobs;

/// <summary>
/// Describes one recurring job as it was actually scheduled, which is the attribute's arguments with any configuration
/// override already applied rather than what the class alone declares.
/// </summary>
///
/// <author>Almighty-Shogun</author>
/// <since>Unreleased</since>
public sealed record RecurringJobInfo
{
    /// <summary>
    /// The stable Hangfire recurring job id, unique across the application.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    public required string JobId { get; init; }

    /// <summary>
    /// The cron expression the job is scheduled with, in the standard five-field format.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    public required string CronExpression { get; init; }

    /// <summary>
    /// The class implementing the job, which is the concrete type rather than <see cref="IRecurringJob"/>.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    public required Type JobType { get; init; }

    /// <summary>
    /// The time zone the cron expression is evaluated in, or <c>null</c> for UTC.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    public required string? TimeZone { get; init; }

    /// <summary>
    /// The queue the job is enqueued on, or <c>null</c> for the Hangfire default.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    public required string? Queue { get; init; }
}
