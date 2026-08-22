namespace AlmightyShogun.Hangfire.RecurringJobs;

/// <summary>
/// Replaces what one job's attribute declares, so a schedule can differ per environment without a rebuild.
/// </summary>
///
/// <remarks>
/// Every property is nullable and an unset one keeps the attribute's value, so a section only has to name what changes.
/// Overridden values are validated exactly like declared ones, which means a bad cron expression here stops the host.
/// </remarks>
///
/// <author>Almighty-Shogun</author>
/// <since>Unreleased</since>
public sealed record RecurringJobOverride
{
    /// <summary>
    /// Gets whether the job is scheduled, outranking both the attribute and <c>EnabledByDefault</c>. This is the value that
    /// turns a job off in one environment and leaves it on everywhere else.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    public bool? Enabled { get; init; }

    /// <summary>
    /// Gets the cron expression to schedule with instead of the declared one, for retuning a schedule per environment or
    /// without a deployment.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    public string? CronExpression { get; init; }

    /// <summary>
    /// Gets the time zone to evaluate the expression in instead of the declared one. It cannot be cleared back to UTC, since
    /// an unset value means the attribute wins.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    public string? TimeZone { get; init; }

    /// <summary>
    /// Gets the queue to enqueue on instead of the declared one, for steering a job onto a queue only some environments
    /// have a server listening on.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    public string? Queue { get; init; }
}
