namespace AlmightyShogun.Hangfire.RecurringJobs;

/// <summary>
/// Marks a recurring Hangfire job class and defines how it is scheduled.
/// It has no effect on a class that does not implement <see cref="IRecurringJob"/>, since the scan only looks for that contract.
/// </summary>
///
/// <param name="jobId">The id the schedule is stored under. Must be unique across the application.</param>
/// <param name="cronExpression">
/// The schedule, as a cron expression in the standard five-field format. <see cref="CronSchedules"/> holds the common ones.
/// </param>
///
/// <author>Almighty-Shogun</author>
/// <since>2.2.0</since>
[AttributeUsage(AttributeTargets.Class, Inherited = false)]
public sealed class RecurringJobAttribute(string jobId, string cronExpression) : Attribute
{
    /// <summary>
    /// The id the schedule is stored under, which must be unique across the application.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>2.2.0</since>
    public string JobId { get; } = jobId;

    /// <summary>
    /// The cron expression the schedule uses, in the standard five-field format.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>2.2.0</since>
    public string CronExpression { get; } = cronExpression;

    /// <summary>
    /// The time zone the cron expression is evaluated in. Defaults to UTC when unset.
    /// </summary>
    ///
    /// <remarks>
    /// Without this, every expression is interpreted as UTC, so a job written to run at 3am runs an hour off for half
    /// the year anywhere that observes daylight saving.
    /// </remarks>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>4.0.0</since>
    public string? TimeZone { get; set; }

    /// <summary>
    /// The Hangfire queue the job is enqueued on, or <c>null</c> for the Hangfire default.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>4.0.0</since>
    public string? Queue { get; set; }

    /// <summary>
    /// Whether the job is scheduled at all. Set it to <c>false</c> to park a job without deleting the class, which, unless a
    /// per-job override turns it back on, also removes whatever schedule is stored under its job id when the host starts, for
    /// as long as <see cref="RecurringJobSettings.RemoveParkedJobs"/> is set.
    /// </summary>
    ///
    /// <remarks>
    /// Leaving it alone is not the same as setting it to <c>true</c>: an untouched job declares nothing and defers to
    /// configuration, as <see cref="RecurringJobSettings"/> describes. The distinction is carried by <c>DeclaredEnabled</c>
    /// because a nullable type cannot be an attribute argument, so this has to present as <see cref="bool"/> while
    /// recording whether it was ever assigned.
    /// </remarks>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>4.0.0</since>
    public bool Enabled
    {
        get => DeclaredEnabled ?? true;
        set => DeclaredEnabled = value;
    }

    /// <summary>
    /// What the class actually declared, with <c>null</c> meaning the job never mentioned <see cref="Enabled"/> and so
    /// defers to configuration. Written only by that setter, which is why it is private to set rather than assigned anywhere.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>4.0.0</since>
    internal bool? DeclaredEnabled { get; private set; }
}
