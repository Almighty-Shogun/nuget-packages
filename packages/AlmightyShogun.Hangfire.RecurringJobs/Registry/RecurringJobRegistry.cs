using Microsoft.Extensions.Options;

namespace AlmightyShogun.Hangfire.RecurringJobs;

/// <summary>
/// Runs the attribute scan once and holds its result for the scheduler and for application code.
/// </summary>
///
/// <param name="sources">The assemblies to scan, supplied by the registration call.</param>
/// <param name="settings">
/// The <c>RecurringJobs</c> options. Their per-job entries override what an attribute declares, while <c>EnabledByDefault</c>
/// sits beneath one. They carry the bound section only when the registration call was given a configuration, and their
/// defaults otherwise.
/// </param>
///
/// <exception cref="ArgumentNullException">
/// A job's attribute declares a <c>null</c> job id, which fails while the per-job overrides are looked up, before the job
/// id is checked at all.
/// </exception>
/// <exception cref="InvalidOperationException">
/// A job declares an empty or whitespace job id; a job declares or is overridden with an empty or invalid cron expression,
/// or an invalid time zone; two jobs share a job id; or an override names a job id nothing declares. Because the scheduler
/// resolves this type, the failure surfaces while the host starts and prevents it from starting.
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
