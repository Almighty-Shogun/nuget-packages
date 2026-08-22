namespace AlmightyShogun.Hangfire.RecurringJobs;

/// <summary>
/// Reports the recurring jobs the application scheduled.
/// </summary>
///
/// <remarks>
/// Resolve this to answer what is scheduled, on a diagnostics endpoint or a startup log line. It describes the schedule
/// only: whether a job is running, when it last ran, and whether it succeeded all live in Hangfire storage and are read
/// through Hangfire's own monitoring API.
/// </remarks>
///
/// <author>Almighty-Shogun</author>
/// <since>Unreleased</since>
public interface IRecurringJobRegistry
{
    /// <summary>
    /// Gets the jobs the attribute scan accepted, which is what was handed to Hangfire rather than what a fresh scan would
    /// find. Jobs whose attribute sets <c>Enabled</c> to <c>false</c> are absent, and the order is discovery order.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    IReadOnlyList<RecurringJobInfo> Jobs { get; }
}
