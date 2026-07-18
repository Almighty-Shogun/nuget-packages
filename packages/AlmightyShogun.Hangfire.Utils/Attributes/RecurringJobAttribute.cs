namespace AlmightyShogun.Hangfire.Utils;

/// <summary>
/// Marks a recurring Hangfire job class and defines its recurring job id and cron expression.
/// </summary>
///
/// <param name="jobId">The stable Hangfire recurring job id.</param>
/// <param name="cronExpression">The cron expression used when scheduling the job.</param>
///
/// <author>Almighty-Shogun</author>
/// <since>2.2.0</since>
[AttributeUsage(AttributeTargets.Class)]
public sealed class RecurringJobAttribute(string jobId, string cronExpression) : Attribute
{
    /// <summary>
    /// Gets the stable Hangfire recurring job id.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>2.2.0</since>
    public string JobId { get; } = jobId;

    /// <summary>
    /// Gets the cron expression used when scheduling the job.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>2.2.0</since>
    public string CronExpression { get; } = cronExpression;
}
