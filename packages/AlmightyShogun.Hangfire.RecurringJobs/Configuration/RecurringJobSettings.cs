namespace AlmightyShogun.Hangfire.RecurringJobs;

/// <summary>
/// Configures recurring job enablement and per-job overrides.
/// </summary>
///
/// <author>Almighty-Shogun</author>
/// <since>4.0.0</since>
public sealed record RecurringJobSettings
{
    /// <summary>
    /// The enablement a job falls back on when neither its override nor attribute specifies one.   
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>4.0.0</since>
    public bool EnabledByDefault { get; init; } = true;

    /// <summary>
    /// Whether parked jobs are removed from Hangfire storage when the host starts.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    public bool RemoveParkedJobs { get; init; } = false;

    /// <summary>
    /// The per-job overrides, keyed by job id.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>4.0.0</since>
    public IReadOnlyDictionary<string, RecurringJobOverride> Jobs { get; init; } = new Dictionary<string, RecurringJobOverride>();
}
