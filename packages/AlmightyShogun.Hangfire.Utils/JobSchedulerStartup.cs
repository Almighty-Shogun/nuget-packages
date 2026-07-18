using Hangfire;
using Microsoft.Extensions.Hosting;

namespace AlmightyShogun.Hangfire.Utils;

/// <summary>
/// Schedules discovered recurring jobs when the host starts.
/// </summary>
///
/// <author>Almighty-Shogun</author>
/// <since>2.2.0</since>
internal sealed class JobSchedulerStartup : IHostedService
{
    /// <summary>
    /// Stores the Hangfire recurring job manager used to register schedules.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>2.2.0</since>
    private readonly IRecurringJobManager _recurring;

    /// <summary>
    /// Stores the recurring job metadata discovered during service registration.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>2.2.0</since>
    private readonly IEnumerable<RecurringJob> _jobs;

    /// <summary>
    /// Creates a hosted startup service that registers recurring jobs with Hangfire.
    /// </summary>
    ///
    /// <param name="recurring">The Hangfire manager used to add or update recurring jobs.</param>
    /// <param name="jobs">The recurring jobs to schedule when the host starts.</param>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>2.2.0</since>
    public JobSchedulerStartup(IRecurringJobManager recurring, IEnumerable<RecurringJob> jobs)
    {
        _recurring = recurring;
        _jobs = jobs;
    }

    /// <inheritdoc />
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>2.2.0</since>
    public Task StartAsync(CancellationToken cancellationToken)
    {
        foreach (RecurringJob job in _jobs)
        {
            _recurring.AddOrUpdate(job.Name, job.ExecutionMethod, job.CronExpression);
        }

        return Task.CompletedTask;
    }

    /// <inheritdoc />
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>2.2.0</since>
    public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
}
