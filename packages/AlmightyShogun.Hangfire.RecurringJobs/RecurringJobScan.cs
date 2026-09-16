namespace AlmightyShogun.Hangfire.RecurringJobs;

/// <summary>
/// Contains the recurring jobs to schedule and the ids of parked jobs.
/// </summary>
///
/// <param name="Jobs">The recurring jobs to schedule.</param>
/// <param name="ParkedJobIds"> The ids of disabled recurring jobs.</param>
///
/// <author>Almighty-Shogun</author>
/// <since>Unreleased</since>
internal sealed record RecurringJobScan(IReadOnlyList<RecurringJobInfo> Jobs, IReadOnlyList<string> ParkedJobIds);
