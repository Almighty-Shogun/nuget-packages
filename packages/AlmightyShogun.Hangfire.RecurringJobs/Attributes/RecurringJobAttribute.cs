namespace AlmightyShogun.Hangfire.RecurringJobs;

/// <summary>
/// Marks a recurring Hangfire job class and defines how it is scheduled.
/// 
/// </summary>
/// <author>Almighty-Shogun</author>
/// <since>2.2.0</since>
[AttributeUsage(AttributeTargets.Class, Inherited = false)]
public sealed class RecurringJobAttribute : Attribute
{
    /// <summary>
    /// Creates a recurring job declaration.
    /// </summary>
    ///
    /// <param name="jobId">The recurring job id. Must be unique across the application.</param>
    /// <param name="cronExpression">
    /// The five- or six-field cron expression. <see cref="CronSchedules"/> contains common schedules.
    /// </param>
    ///
    /// <exception cref="ArgumentException">
    /// <paramref name="jobId"/> or <paramref name="cronExpression"/> is null, empty, or whitespace.
    /// </exception>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>2.2.0</since>
    public RecurringJobAttribute(string jobId, string cronExpression)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(jobId);
        ArgumentException.ThrowIfNullOrWhiteSpace(cronExpression);

        JobId = jobId;
        CronExpression = cronExpression;
    }

    /// <summary>
    /// The recurring job id.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>2.2.0</since>
    public string JobId { get; }

    /// <summary>
    /// The five- or six-field cron expression.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>2.2.0</since>
    public string CronExpression { get; }

    /// <summary>
    /// The time zone the cron expression is evaluated in, or <c>null</c> for UTC.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>4.0.0</since>
    public string? TimeZone { get; set; }

    /// <summary>
    /// The Hangfire queue the job is enqueued on, or <c>null</c> for the default queue.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>4.0.0</since>
    public string? Queue { get; set; }

    /// <summary>
    /// Whether the job is enabled.
    /// </summary>
    ///
    /// <remarks>
    /// When not explicitly set, enablement is determined by <see cref="RecurringJobSettings"/>.
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
    /// The explicitly declared enablement, or <c>null</c> when <see cref="Enabled"/> was not set.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>4.0.0</since>
    internal bool? DeclaredEnabled { get; private set; }
}
