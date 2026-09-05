namespace AlmightyShogun.Hangfire.RecurringJobs;

/// <summary>
/// Contract for a recurring Hangfire job. Implementing it is not enough on its own: the class is only scheduled when it
/// also carries <see cref="RecurringJobAttribute"/> and its assembly is passed to one of the registration overloads.
/// </summary>
///
/// <remarks>
/// Registration is scoped, and Hangfire's job activator resolves the class from its own scope for every run, so a
/// constructor dependency may be a scoped service such as a database context, and no field survives from one run to the
/// next. An exception that escapes a run is left to Hangfire, whose own retry policy decides what happens next; this
/// package installs no job filter of its own.
/// </remarks>
///
/// <author>Almighty-Shogun</author>
/// <since>Unreleased</since>
public interface IRecurringJob
{
    /// <summary>
    /// Executes the recurring Hangfire job. Implement it publicly rather than explicitly, since Hangfire invokes the method
    /// by reflection and an explicit implementation is private.
    /// </summary>
    ///
    /// <param name="cancellationToken">
    /// Hangfire supplies the running server's own token here, signalled when the job is aborted or the server is shutting
    /// down. A job with nothing to unwind can ignore it, but Hangfire waits on a long-running one that never observes it,
    /// which delays shutdown.
    /// </param>
    ///
    /// <returns>A task that represents the asynchronous execution of the recurring job.</returns>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    Task RunAsync(CancellationToken cancellationToken);
}
