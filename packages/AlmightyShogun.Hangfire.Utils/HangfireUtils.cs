using Hangfire.Common;
using System.Reflection;
using AlmightyShogun.Hangfire.Utils.Attributes;

namespace AlmightyShogun.Hangfire.Utils;

public static class HangfireUtils
{
    /// <summary>
    /// Retrieves all recurring jobs marked with the <see cref="RecurringJobAttribute"/> defined within the application.
    /// </summary>
    /// 
    /// <returns>A list of <see cref="Type"/> instances representing the recurring job type.</returns>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>2.2.0</since>
    internal static IEnumerable<Type> GetRecurringJobTypes() => Assembly.GetExecutingAssembly()
        .GetTypes()
        .Where(t => t.GetCustomAttribute<RecurringJobAttribute>() != null);
    
    /// <summary>
    /// Retrieves all recurring jobs marked with the <see cref="RecurringJobAttribute"/> defined within the application.
    /// </summary>
    /// 
    /// <returns>A list of <see cref="RecurringJob"/> instances representing the recurring job.</returns>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>2.2.0</since>
    public static IEnumerable<RecurringJob> GetRecurringJobs() => AppDomain.CurrentDomain.GetAssemblies()
        .SelectMany(a => a.GetTypes())
        .Where(t => t.GetCustomAttribute<RecurringJobAttribute>() is not null)
        .Select(m => new RecurringJob(
            m.GetCustomAttribute<RecurringJobAttribute>()!.JobId, 
            m.GetCustomAttribute<RecurringJobAttribute>()!.CronExpression,
            new Job(m, m.GetMethod("RunAsync")!)
        )).ToList();
}
