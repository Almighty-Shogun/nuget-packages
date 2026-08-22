namespace AlmightyShogun.Hangfire.RecurringJobs;

/// <summary>
/// Base class for a recurring Hangfire job. Inheriting it is not enough on its own: the class is only scheduled when it
/// also carries <see cref="RecurringJobAttribute"/> and its assembly is passed to one of the registration overloads.
/// </summary>
///
/// <remarks>
/// Jobs are registered scoped and resolved from a fresh scope for every run, so a constructor dependency may be a scoped
/// service such as a database context, and no field survives from one run to the next. An exception that escapes a run is
/// handled by Hangfire, which retries the job on its own policy rather than dropping it.
/// </remarks>
///
/// <author>Almighty-Shogun</author>
/// <since>Unreleased</since>
public abstract class RecurringJobBase
{
    /// <summary>
    /// Executes the recurring Hangfire job.
    /// </summary>
    ///
    /// <param name="cancellationToken">
    /// Signalled when the job is aborted or the server is shutting down. A job with nothing to unwind can ignore it, but a
    /// long-running one that never observes it delays shutdown until Hangfire gives up waiting.
    /// </param>
    ///
    /// <returns>A task that represents the asynchronous execution of the recurring job.</returns>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    public abstract Task RunAsync(CancellationToken cancellationToken);
}
