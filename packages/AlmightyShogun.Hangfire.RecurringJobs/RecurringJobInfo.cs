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
    /// Gets the stable Hangfire recurring job id, unique across the application and never overridable.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    public required string JobId { get; init; }

    /// <summary>
    /// Gets the cron expression the job is scheduled with, validated during discovery.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    public required string CronExpression { get; init; }

    /// <summary>
    /// Gets the class implementing the job, registered scoped so that Hangfire's job activator resolves it from its own
    /// scope on every run.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    public required Type JobType { get; init; }

    /// <summary>
    /// Gets the time zone the cron expression is evaluated in, or <c>null</c> for UTC.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    public required string? TimeZone { get; init; }

    /// <summary>
    /// Gets the queue the job is enqueued on, or <c>null</c> for the Hangfire default.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    public required string? Queue { get; init; }
}
