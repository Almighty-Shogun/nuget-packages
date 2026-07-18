namespace AlmightyShogun.Hangfire.Utils;

/// <summary>
/// Base class for recurring Hangfire jobs discovered and scheduled by Hangfire Utils.
/// </summary>
///
/// <author>Almighty-Shogun</author>
/// <since>Unreleased</since>
public abstract class RecurringJobBase
{
    /// <summary>
    /// Executes the recurring Hangfire job.
    /// </summary>
    ///
    /// <returns>A task that represents the asynchronous execution of the recurring job.</returns>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    public abstract Task RunAsync();
}
