using Microsoft.Extensions.Options;

namespace AlmightyShogun.Hangfire.RecurringJobs;

/// <summary>
/// Runs the attribute scan once and holds its result for the scheduler and for application code.
/// </summary>
///
/// <param name="sources">The assemblies to scan, supplied by the registration call.</param>
/// <param name="settings">The bound <c>RecurringJobs</c> section, applied on top of what each attribute declares.</param>
///
/// <exception cref="InvalidOperationException">
/// A job declares or is overridden with an invalid cron expression or time zone, two jobs share a job id, or an override
/// names a job id nothing declares. Because the scheduler resolves this type, the failure surfaces while the host starts
/// and prevents it from starting.
/// </exception>
///
/// <remarks>
/// Registered as a singleton, so the scan runs once no matter how many callers resolve the registry. Configuration is read
/// through <see cref="IOptions{TOptions}"/> rather than the reloading variants, since a schedule already handed to Hangfire
/// does not change when the file does.
/// </remarks>
///
/// <author>Almighty-Shogun</author>
/// <since>Unreleased</since>
internal sealed class RecurringJobRegistry(RecurringJobSources sources, IOptions<RecurringJobSettings> settings) : IRecurringJobRegistry
{
    /// <inheritdoc />
    public IReadOnlyList<RecurringJobInfo> Jobs { get; } = RecurringJobDiscovery.GetRecurringJobs(sources.Assemblies, settings.Value);
}
