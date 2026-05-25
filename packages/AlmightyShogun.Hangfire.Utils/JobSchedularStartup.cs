using Hangfire;
using Microsoft.Extensions.Hosting;

namespace AlmightyShogun.Hangfire.Utils;

public class JobSchedulerStartup(IRecurringJobManager recurring, IEnumerable<RecurringJob> jobs) : IHostedService
{
    /// <inheritdoc/>
    public Task StartAsync(CancellationToken cancellationToken)
    {
        foreach (RecurringJob job in jobs)
        {
            recurring.AddOrUpdate(job.Name, job.ExecutionMethod, job.CronExpression);
        }

        return Task.CompletedTask;
    }

    /// <inheritdoc/>
    public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
}
