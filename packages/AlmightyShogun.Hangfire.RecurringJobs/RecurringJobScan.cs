namespace AlmightyShogun.Hangfire.RecurringJobs;

/// <summary>
/// Carries both halves of one attribute scan, so a single pass can report what to schedule and what to unschedule.
/// </summary>
///
/// <param name="Jobs">The jobs to hand to Hangfire, in the order the scan found them.</param>
/// <param name="ParkedJobIds">
/// The ids of the jobs the scan found, validated, and then dropped because they resolved to disabled, which
/// <see cref="JobSchedulerStartup"/> unschedules while <see cref="RecurringJobSettings.RemoveParkedJobs"/> is set. An id no
/// discovered job declares is not in here.
/// </param>
///
/// <author>Almighty-Shogun</author>
/// <since>Unreleased</since>
internal sealed record RecurringJobScan(IReadOnlyList<RecurringJobInfo> Jobs, IReadOnlyList<string> ParkedJobIds);
