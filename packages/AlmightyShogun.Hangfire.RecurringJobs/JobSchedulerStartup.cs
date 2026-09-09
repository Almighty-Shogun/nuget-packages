using Hangfire;
using Hangfire.Common;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;

namespace AlmightyShogun.Hangfire.RecurringJobs;

/// <summary>
/// Applies the scan to Hangfire when the host starts, removing parked schedules before adding or updating active ones.
/// </summary>
///
/// <param name="recurringJobManager">The Hangfire manager used to add, update and remove recurring jobs.</param>
/// <param name="registry">
/// The singleton holding the scan result, taken as the concrete type because <see cref="RecurringJobRegistry.ParkedJobIds"/>
/// is not on <see cref="IRecurringJobRegistry"/>.
/// </param>
/// <param name="settings">
/// The <c>RecurringJobs</c> options, read only for <see cref="RecurringJobSettings.RemoveParkedJobs"/>. The registry has
/// already applied the rest of the section to produce the scan.
/// </param>
///
/// <remarks>
/// Scheduling is an <c>AddOrUpdate</c> against a stable job id, which is how Hangfire re-declares an existing schedule
/// rather than adding a second one, so a restart does not duplicate anything. Removal is then the same idea in reverse: a
/// job the scan parked has its entry deleted, so switching one off leaves nothing behind in a durable store. Only the ids
/// this scan saw and parked are removed, so a job renamed or deleted in the code leaves its old entry alone. That deletion
/// happens only while <see cref="RecurringJobSettings.RemoveParkedJobs"/> is set, since the entry belongs to the storage
/// rather than to this instance, and anything else pointed at that storage loses whatever it wrote under a parked id. The
/// removals run first on purpose: a job that fails to register throws out of <c>StartAsync</c>, and doing them second would
/// leave a parked job scheduled on a durable store that a server elsewhere is already reading. No id is in both halves of the
/// scan, since a duplicate job id is rejected there, so the order costs nothing.
/// </remarks>
///
/// <author>Almighty-Shogun</author>
/// <since>2.2.0</since>
internal sealed class JobSchedulerStartup(
    IRecurringJobManager recurringJobManager,
    RecurringJobRegistry registry,
    IOptions<RecurringJobSettings> settings
) : IHostedService
{
    /// <inheritdoc />
    ///
    /// <exception cref="ArgumentException">
    /// A job's merged queue name is not one Hangfire accepts. Nothing checks it during the scan, so
    /// <see cref="RecurringJobDiscovery.CreateExecutionMethod"/> surfaces it here and the host does not start.
    /// </exception>
    /// <exception cref="InvalidOperationException">
    /// A job type exposes no public run method taking a cancellation token, which
    /// <see cref="RecurringJobDiscovery.CreateExecutionMethod"/> also surfaces here rather than during the scan.
    /// </exception>
    public Task StartAsync(CancellationToken cancellationToken)
    {
        if (settings.Value.RemoveParkedJobs)
            foreach (string jobId in registry.ParkedJobIds)
                recurringJobManager.RemoveIfExists(jobId);

        foreach (RecurringJobInfo job in registry.Jobs)
        {
            RecurringJobOptions options = new();

            if (job.TimeZone is not null)
                options.TimeZone = TimeZoneInfo.FindSystemTimeZoneById(job.TimeZone);

            Job executionMethod = RecurringJobDiscovery.CreateExecutionMethod(job);

            recurringJobManager.AddOrUpdate(job.JobId, executionMethod, job.CronExpression, options);
        }

        return Task.CompletedTask;
    }

    /// <inheritdoc />
    public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
}
