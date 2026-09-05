namespace AlmightyShogun.Hangfire.RecurringJobs;

/// <summary>
/// Reports the recurring jobs the application scheduled.
/// </summary>
///
/// <remarks>
/// Resolve this to answer what is scheduled, on a diagnostics endpoint or a startup log line. It describes the schedule
/// only and carries no run state, so whether a job is running, when it last ran, and whether it succeeded live in Hangfire
/// storage and are read through Hangfire's own monitoring API.
/// </remarks>
///
/// <author>Almighty-Shogun</author>
/// <since>Unreleased</since>
public interface IRecurringJobRegistry
{
    /// <summary>
    /// Gets the jobs the scan kept, which are the ones the scheduler hands to Hangfire, rather than what a fresh scan would
    /// find. Nothing here records whether the handover actually happened. A job the attribute, the
    /// configuration section, or <see cref="RecurringJobSettings.EnabledByDefault"/> left disabled is absent even though it
    /// was discovered and validated, so this reports what runs here rather than what the assemblies declare.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    IReadOnlyList<RecurringJobInfo> Jobs { get; }
}
