namespace AlmightyShogun.Hangfire.RecurringJobs;

/// <summary>
/// Describes one recurring job the attribute scan accepted, which is the same value the scheduler hands to Hangfire.
/// </summary>
///
/// <param name="JobId">The stable Hangfire recurring job id, unique across the application.</param>
/// <param name="CronExpression">The cron expression the job is scheduled with, validated during discovery.</param>
/// <param name="JobType">The class implementing the job, resolved from a fresh scope on every run.</param>
/// <param name="TimeZone">The time zone the cron expression is evaluated in, or <c>null</c> for UTC.</param>
/// <param name="Queue">The queue the job is enqueued on, or <c>null</c> for the Hangfire default.</param>
///
/// <author>Almighty-Shogun</author>
/// <since>Unreleased</since>
public sealed record RecurringJobInfo(
    string JobId,
    string CronExpression,
    Type JobType,
    string? TimeZone,
    string? Queue
);
