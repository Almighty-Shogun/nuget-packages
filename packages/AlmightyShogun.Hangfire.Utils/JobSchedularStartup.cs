using Hangfire;
using Microsoft.Extensions.Hosting;

namespace AlmightyShogun.Hangfire.Utils;

public class JobSchedulerStartup(IRecurringJobManager recurring) : IHostedService
{
    /// <inheritdoc/>
    public Task StartAsync(CancellationToken cancellationToken)
    {
        foreach (RecurringJob job in HangfireUtils.GetRecurringJobs())
        {
            recurring.AddOrUpdate(job.Name, job.Method, job.CronExpression);
        }

        return Task.CompletedTask;
    }

    /// <inheritdoc/>
    public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
}
