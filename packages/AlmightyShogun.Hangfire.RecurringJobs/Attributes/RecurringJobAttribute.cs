namespace AlmightyShogun.Hangfire.RecurringJobs;

/// <summary>
/// Marks a recurring Hangfire job class and defines how it is scheduled. It has no effect on a class that does not
/// inherit <see cref="RecurringJobBase"/>, since the scan only looks at that hierarchy.
/// </summary>
///
/// <param name="jobId">
/// The id the schedule is stored under. Must be unique across the application, since a second job claiming it stops the host.
/// </param>
/// <param name="cronExpression">
/// The schedule, as a cron expression Cronos accepts. <see cref="CronSchedules"/> holds the common ones.
/// </param>
///
/// <author>Almighty-Shogun</author>
/// <since>2.2.0</since>
[AttributeUsage(AttributeTargets.Class)]
public sealed class RecurringJobAttribute(string jobId, string cronExpression) : Attribute
{
    /// <summary>
    /// Gets the id the schedule is stored under. Changing it on a job that has already run leaves the previous schedule in
    /// storage under the old id, where nothing removes it.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>2.2.0</since>
    public string JobId { get; } = jobId;

    /// <summary>
    /// Gets the cron expression the schedule uses. It is parsed while the host starts, so a malformed expression stops the
    /// application instead of leaving a job that quietly never fires.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>2.2.0</since>
    public string CronExpression { get; } = cronExpression;

    /// <summary>
    /// Gets or sets the time zone the cron expression is evaluated in. Defaults to UTC when unset.
    /// </summary>
    ///
    /// <remarks>
    /// Without this, every expression is interpreted as UTC, so a job written to run at 3am runs an hour off for half
    /// the year anywhere that observes daylight saving.
    /// </remarks>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    public string? TimeZone { get; set; }

    /// <summary>
    /// Gets or sets the Hangfire queue the job is enqueued on. Naming a queue no server listens on leaves the job enqueued
    /// and never processed, which looks the same from the outside as a job that never fired.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    public string? Queue { get; set; }

    /// <summary>
    /// Gets or sets whether the job is scheduled at all. Set it to <c>false</c> to park a job without deleting the class.
    /// A parked job is still validated, so it cannot hide a broken cron expression until someone switches it back on.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    public bool Enabled { get; set; } = true;
}
