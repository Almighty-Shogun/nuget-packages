namespace AlmightyShogun.Hangfire.Utils;

public interface IRecurringJob
{
    /// <summary>
    /// Executes the recurring Hangfire job.
    /// </summary>
    /// 
    /// <returns>A task that represents the asynchronous execution of the recurring job.</returns>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>3.0.0</since>
    Task RunAsync();
}
