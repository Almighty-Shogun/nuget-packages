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
/// The singleton holding the scan result. Resolving it is what runs the scan, so an invalid attribute fails here rather
/// than at the first cron tick.
/// </param>
///
/// <remarks>
/// Scheduling is an add-or-update against a stable job id, so a restart re-declares the same schedules instead of
/// duplicating them. A job removed from the code is not removed from storage, which only matters for a durable store.
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
