namespace AlmightyShogun.Hangfire.RecurringJobs;

/// <summary>
/// Represents the <c>RecurringJobs</c> configuration section, which adjusts what the attribute scan found.
/// </summary>
///
/// <remarks>
/// The section is optional and every value has a default, so an application that schedules the same jobs everywhere never
/// has to declare it. Binding it in <c>appsettings.Development.json</c> is what makes a job differ between environments.
/// </remarks>
///
/// <author>Almighty-Shogun</author>
/// <since>Unreleased</since>
public sealed record RecurringJobSettings
{
    /// <summary>
    /// Gets whether a job whose attribute does not state either way is scheduled. Setting it to <c>false</c> in one
    /// environment parks everything except the jobs that opt in explicitly.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    public bool EnabledByDefault { get; init; } = true;

    /// <summary>
    /// Gets the per-job overrides, keyed by job id. Matching ignores case, and a key naming a job the scan did not find
    /// stops the host rather than being ignored, since that is nearly always a typo in a job id.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    public IReadOnlyDictionary<string, RecurringJobOverride> Jobs { get; init; } = new Dictionary<string, RecurringJobOverride>();
}
