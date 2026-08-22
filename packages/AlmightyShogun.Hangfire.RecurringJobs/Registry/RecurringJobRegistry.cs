namespace AlmightyShogun.Hangfire.RecurringJobs;

/// <summary>
/// Runs the attribute scan once and holds its result for the scheduler and for application code.
/// </summary>
///
/// <param name="sources">The assemblies to scan, supplied by the registration call.</param>
///
/// <exception cref="InvalidOperationException">
/// A job declares an invalid cron expression or time zone, or two jobs share a job id. Because the scheduler resolves this
/// type, the failure surfaces while the host starts and prevents it from starting.
/// </exception>
///
/// <remarks>
/// Registered as a singleton, so the scan runs once no matter how many callers resolve the registry.
/// </remarks>
///
/// <author>Almighty-Shogun</author>
/// <since>Unreleased</since>
internal sealed class RecurringJobRegistry(RecurringJobSources sources) : IRecurringJobRegistry
{
    /// <inheritdoc />
    public IReadOnlyList<RecurringJobInfo> Jobs { get; } = RecurringJobDiscovery.GetRecurringJobs(sources.Assemblies);
}
