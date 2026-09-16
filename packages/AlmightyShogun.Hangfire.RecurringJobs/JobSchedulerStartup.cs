using Hangfire;
using Hangfire.Common;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;

namespace AlmightyShogun.Hangfire.RecurringJobs;

/// <summary>
/// Applies recurring job registrations when the host starts.
/// </summary>
///
/// <param name="recurringJobManager">The Hangfire recurring job manager.</param>
/// <param name="registry"> The recurring job registry. </param>
/// <param name="settings"> The recurring job settings. </param>
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
