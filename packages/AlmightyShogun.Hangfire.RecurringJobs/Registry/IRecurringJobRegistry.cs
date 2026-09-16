namespace AlmightyShogun.Hangfire.RecurringJobs;

/// <summary>
/// Provides the recurring jobs scheduled by the application.
/// </summary>
///
/// <author>Almighty-Shogun</author>
/// <since>4.0.0</since>
public interface IRecurringJobRegistry
{
    /// <summary>
    /// The recurring jobs scheduled by the application.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>4.0.0</since>
    IReadOnlyList<RecurringJobInfo> Jobs { get; }
}
