using Microsoft.Extensions.Options;

namespace AlmightyShogun.Hangfire.RecurringJobs;

/// <summary>
/// Runs the attribute scan once and holds its result for the scheduler and for application code.
/// </summary>
///
/// <param name="sources">
/// One entry per registration call, since the call registers its own instance rather than replacing what an earlier one
/// registered. The scan runs over the union, deduplicated by assembly identity because
/// <see cref="AlmightyShogun.Utils.TypeDiscovery.FindAssignableTypes{T}(System.Reflection.Assembly[])"/> enumerates the
/// array it is given without collapsing repeats, so an assembly two calls both name would yield each of its job types twice
/// and stop the host on the duplicate job id <see cref="RecurringJobDiscovery.GetRecurringJobs"/> rejects.
/// </param>
/// <param name="settings">
/// The <c>RecurringJobs</c> options. They carry the bound section only when the registration call was given a
/// configuration, and their defaults otherwise.
/// </param>
///
/// <exception cref="ArgumentNullException">
/// A job's attribute declares a <c>null</c> job id, which fails while the per-job overrides are looked up, before the job
/// id is checked at all.
/// </exception>
/// <exception cref="InvalidOperationException">
/// A job declares an empty or whitespace job id; a job declares or is overridden with an empty or invalid cron expression,
/// or an invalid time zone; two jobs share a job id; or an override names a job id nothing declares. Because the scheduler
/// resolves this type, the failure surfaces while the host starts and prevents it from starting.
/// </exception>
///
/// <remarks>
/// Registered as a singleton under both this type and <see cref="IRecurringJobRegistry"/>, resolving to one instance, so the
/// scan runs once no matter how many callers resolve either. Configuration is read through <see cref="IOptions{TOptions}"/>
/// rather than the reloading variants, since a schedule already handed to Hangfire does not change when the file does.
/// </remarks>
///
/// <author>Almighty-Shogun</author>
/// <since>4.0.0</since>
internal sealed class RecurringJobRegistry(IEnumerable<RecurringJobSources> sources, IOptions<RecurringJobSettings> settings)
    : IRecurringJobRegistry
{
    /// <summary>
    /// The one scan result both members read, so the parked ids belong to the same pass that produced the scheduled jobs.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    private readonly RecurringJobScan _scan = RecurringJobDiscovery.GetRecurringJobs(
        [.. sources.SelectMany(static source => source.Assemblies).Distinct()],
        settings.Value
    );

    /// <inheritdoc />
    public IReadOnlyList<RecurringJobInfo> Jobs => _scan.Jobs;

    /// <summary>
    /// The ids of the jobs the scan parked, which <see cref="JobSchedulerStartup"/> unschedules while
    /// <see cref="RecurringJobSettings.RemoveParkedJobs"/> is set. It stays off <see cref="IRecurringJobRegistry"/>, so the
    /// scheduler takes this type rather than the interface.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    internal IReadOnlyList<string> ParkedJobIds => _scan.ParkedJobIds;
}
