namespace AlmightyShogun.Hangfire.RecurringJobs;

/// <summary>
/// Represents the <c>RecurringJobs</c> configuration section, which adjusts what the attribute scan found. Whether a job is
/// scheduled is decided by its per-job override first, then by what its attribute declared, then by <c>EnabledByDefault</c>.
/// </summary>
///
/// <remarks>
/// The section is optional and every value has a default, so an application that schedules the same jobs everywhere never
/// has to declare it. Binding it in <c>appsettings.Development.json</c> is what makes a job differ between environments.
/// </remarks>
///
/// <author>Almighty-Shogun</author>
/// <since>4.0.0</since>
public sealed record RecurringJobSettings
{
    /// <summary>
    /// The enablement a job falls back on when nothing else states one. Setting it to <c>false</c> in one environment
    /// parks everything except the jobs that opt in explicitly.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>4.0.0</since>
    public bool EnabledByDefault { get; init; } = true;

    /// <summary>
    /// The per-job overrides, keyed by job id.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>4.0.0</since>
    public IReadOnlyDictionary<string, RecurringJobOverride> Jobs { get; init; } = new Dictionary<string, RecurringJobOverride>();
}
