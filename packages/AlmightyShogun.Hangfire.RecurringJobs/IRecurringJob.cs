namespace AlmightyShogun.Hangfire.RecurringJobs;

/// <summary>
/// Represents a recurring Hangfire job.
/// </summary>
///
/// <author>Almighty-Shogun</author>
/// <since>4.0.0</since>
public interface IRecurringJob
{
    /// <summary>
    /// Executes the recurring job.
    /// </summary>
    ///
    /// <param name="cancellationToken">The cancellation token for the current execution.</param>
    ///
    /// <returns>A task that represents the asynchronous execution.</returns>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>4.0.0</since>
    Task RunAsync(CancellationToken cancellationToken);
}
