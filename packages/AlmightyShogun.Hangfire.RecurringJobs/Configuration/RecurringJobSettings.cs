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
    /// parks everything except the jobs that opt in explicitly, and, while <see cref="RemoveParkedJobs"/> is set, every
    /// job it parks has whatever schedule is stored under its job id removed when the host starts.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>4.0.0</since>
    public bool EnabledByDefault { get; init; } = true;

    /// <summary>
    /// Whether this application deletes the stored schedule of every job it parked when the host starts. Clearing it
    /// leaves those entries alone, so parking a job stops this application scheduling it without touching what anything
    /// else wrote under the same job id.
    /// </summary>
    ///
    /// <remarks>
    /// The deletion is by job id against Hangfire storage, which belongs to everything pointed at that storage rather than
    /// to the instance doing the deleting: another replica, a client role registering the same jobs with
    /// <c>AddCustomHangfire(addServer: false)</c>, an older instance still running through a rolling deploy, and any code
    /// that called <c>RecurringJob.AddOrUpdate</c> under a parked id all lose their entry to it. Clear this wherever the
    /// storage is not this application's alone to rewrite; leaving it set is what makes parking a job take effect on a
    /// durable store instead of leaving the previous schedule running.
    /// </remarks>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    public bool RemoveParkedJobs { get; init; } = true;

    /// <summary>
    /// The per-job overrides, keyed by job id.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>4.0.0</since>
    public IReadOnlyDictionary<string, RecurringJobOverride> Jobs { get; init; } = new Dictionary<string, RecurringJobOverride>();
}
