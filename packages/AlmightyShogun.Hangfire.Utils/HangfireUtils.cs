using Hangfire.Common;
using System.Reflection;

namespace AlmightyShogun.Hangfire.Utils;

/// <summary>
/// Provides discovery helpers for recurring Hangfire job types and their scheduling metadata.
/// </summary>
///
/// <author>Almighty-Shogun</author>
/// <since>2.2.0</since>
internal static class HangfireUtils
{
    /// <summary>
    /// Retrieves recurring job types marked with <see cref="RecurringJobAttribute"/> from the provided assemblies.
    /// </summary>
    ///
    /// <param name="assemblies">The assemblies to scan. If none are provided, all currently loaded application domain assemblies are used.</param>
    ///
    /// <returns>The concrete recurring job types that inherit <see cref="RecurringJobBase"/> and expose a valid <see cref="RecurringJobBase.RunAsync"/> method.</returns>
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
            .Where(t => t is { IsInterface: false, IsAbstract: false } && typeof(RecurringJobBase).IsAssignableFrom(t))
            .Where(t => t.GetCustomAttribute<RecurringJobAttribute>() != null)
            .Where(t =>
            {
                MethodInfo? runMethod = t.GetMethod(nameof(RecurringJobBase.RunAsync), BindingFlags.Public | BindingFlags.Instance);

                return runMethod is not null && runMethod.ReturnType == typeof(Task) && runMethod.GetParameters().Length == 0;
            });
    }

    /// <summary>
    /// Retrieves recurring job metadata from all currently loaded assemblies.
    /// </summary>
    ///
    /// <returns>The recurring job metadata used by the startup service to schedule jobs.</returns>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>2.2.0</since>
    internal static IEnumerable<RecurringJob> GetRecurringJobs() => GetRecurringJobs(AppDomain.CurrentDomain.GetAssemblies());

    /// <summary>
    /// Retrieves recurring job metadata from the provided assemblies.
    /// </summary>
    ///
    /// <param name="assemblies">The assemblies to scan for recurring job types.</param>
    ///
    /// <returns>The recurring job metadata built from each discovered job type and its attribute.</returns>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>3.0.0</since>
    internal static IEnumerable<RecurringJob> GetRecurringJobs(params Assembly[] assemblies) => GetRecurringJobTypes(assemblies)
        .Select(type =>
        {
            RecurringJobAttribute attribute = type.GetCustomAttribute<RecurringJobAttribute>()
                ?? throw new InvalidOperationException($"{type.Name} must define {nameof(RecurringJobAttribute)}.");

            MethodInfo runMethod = type.GetMethod(nameof(RecurringJobBase.RunAsync), BindingFlags.Public | BindingFlags.Instance)
                ?? throw new InvalidOperationException($"{type.Name} must define a public instance method named {nameof(RecurringJobBase.RunAsync)}.");

            return new RecurringJob(attribute.JobId, attribute.CronExpression, new Job(type, runMethod));
        }).ToList();
}
