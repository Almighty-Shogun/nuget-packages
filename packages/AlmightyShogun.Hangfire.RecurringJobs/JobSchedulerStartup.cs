using Hangfire;
using Hangfire.Common;
using Microsoft.Extensions.Hosting;

namespace AlmightyShogun.Hangfire.RecurringJobs;

/// <summary>
/// Hands the discovered jobs to Hangfire when the host starts.
/// </summary>
///
/// <param name="recurringJobManager">The Hangfire manager used to add or update recurring jobs.</param>
/// <param name="registry">
/// The singleton holding the scan result. Resolving it is what runs the scan, so a bad attribute argument or a bad override
/// fails here rather than at the first cron tick.
/// </param>
///
/// <remarks>
/// Scheduling is an <c>AddOrUpdate</c> against a stable job id, which is how Hangfire re-declares an existing schedule
/// rather than adding a second one, so a restart does not duplicate anything. Nothing in this package ever removes a
/// schedule, so a job deleted from the code leaves its entry behind in Hangfire storage, which only matters for a durable
/// store.
/// </remarks>
///
/// <author>Almighty-Shogun</author>
/// <since>2.2.0</since>
internal sealed class JobSchedulerStartup(IRecurringJobManager recurringJobManager, IRecurringJobRegistry registry) : IHostedService
{
    /// <inheritdoc />
    public Task StartAsync(CancellationToken cancellationToken)
    {
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
