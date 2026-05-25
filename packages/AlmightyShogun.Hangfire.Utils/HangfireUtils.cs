using Hangfire.Common;
using System.Reflection;

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
    internal static IEnumerable<Type> GetRecurringJobTypes(params Assembly[] assemblies)
    {
        if (assemblies.Length == 0)
        {
            assemblies = AppDomain.CurrentDomain.GetAssemblies();
        }
        
        return assemblies
            .SelectMany(a => a.GetTypes())
            .Where(t => t is { IsInterface: false, IsAbstract: false } && typeof(IRecurringJob).IsAssignableFrom(t))
            .Where(t => t.GetCustomAttribute<RecurringJobAttribute>() != null)
            .Where(t =>
            {
                MethodInfo? runMethod = t.GetMethod(nameof(IRecurringJob.RunAsync), BindingFlags.Public | BindingFlags.Instance);

                return runMethod is not null && runMethod.ReturnType == typeof(Task) && runMethod.GetParameters().Length == 0;
            });
    }
    
    /// <summary>
    /// Retrieves all recurring jobs marked with the <see cref="RecurringJobAttribute"/> defined within the application.
    /// </summary>
    /// 
    /// <returns>A list of <see cref="RecurringJob"/> instances representing the recurring job.</returns>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>2.2.0</since>
    public static IEnumerable<RecurringJob> GetRecurringJobs() => GetRecurringJobs(AppDomain.CurrentDomain.GetAssemblies());

    /// <summary>
    /// Retrieves all recurring jobs marked with the <see cref="RecurringJobAttribute"/> defined within the provided assemblies.
    /// </summary>
    /// 
    /// <param name="assemblies">The assemblies to scan for recurring job types.</param>
    /// 
    /// <returns>A list of <see cref="RecurringJob"/> instances representing the recurring jobs.</returns>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>3.0.0</since>
    internal static IEnumerable<RecurringJob> GetRecurringJobs(params Assembly[] assemblies) => GetRecurringJobTypes(assemblies)
        .Select(type =>
        {
            RecurringJobAttribute attribute = type.GetCustomAttribute<RecurringJobAttribute>()
                ?? throw new InvalidOperationException($"{type.Name} must define {nameof(RecurringJobAttribute)}.");
            
            MethodInfo runMethod = type.GetMethod(nameof(IRecurringJob.RunAsync), BindingFlags.Public | BindingFlags.Instance)
                ?? throw new InvalidOperationException($"{type.Name} must define a public instance method named {nameof(IRecurringJob.RunAsync)}.");

            return new RecurringJob(attribute.JobId, attribute.CronExpression, new Job(type, runMethod));
        }).ToList();
}
