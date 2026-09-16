using Microsoft.Extensions.Options;

namespace AlmightyShogun.Hangfire.RecurringJobs;

/// <summary>
/// Holds the discovered recurring jobs for the application.
/// </summary>
///
/// <param name="sources"> The registered recurring job sources.</param>
/// <param name="settings"> The recurring job settings. </param>
///
/// <exception cref="InvalidOperationException">
/// The recurring job configuration or discovered jobs are invalid.
/// </exception>
///
/// <author>Almighty-Shogun</author>
/// <since>4.0.0</since>
internal sealed class RecurringJobRegistry(
    IEnumerable<RecurringJobSources> sources,
    IOptions<RecurringJobSettings> settings
) : IRecurringJobRegistry
{
    /// <summary>
    /// The recurring job scan result.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    private readonly RecurringJobScan _scan = RecurringJobDiscovery.GetRecurringJobs(
        [.. sources.SelectMany(static source => source.Assemblies).Distinct()],
        settings.Value
    );

    /// <inheritdoc />
    public IReadOnlyList<RecurringJobInfo> Jobs => _scan.Jobs;

    /// <summary>
    /// The ids of disabled recurring jobs.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    internal IReadOnlyList<string> ParkedJobIds => _scan.ParkedJobIds;
}
