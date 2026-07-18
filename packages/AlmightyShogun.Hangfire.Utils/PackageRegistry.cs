using Hangfire;
using System.Reflection;
using AlmightyShogun.Utils;
using Microsoft.Extensions.DependencyInjection;

namespace AlmightyShogun.Hangfire.Utils;

/// <summary>
/// Registers Hangfire infrastructure and recurring job services.
/// </summary>
///
/// <author>Almighty-Shogun</author>
/// <since>2.2.0</since>
public static class PackageRegistry
{
    /// <param name="serviceCollection">The service collection used to register dependencies related to the Hangfire functionality.</param>
    extension(IServiceCollection serviceCollection)
    {
        /// <summary>
        /// Registers Hangfire, in-memory storage, and the Hangfire background processing server.
        /// </summary>
        ///
        /// <param name="compatibilityLevel">The Hangfire data compatibility level to apply. Defaults to <see cref="CompatibilityLevel.Version_180"/>.</param>
        ///
        /// <returns>The <see cref="IServiceCollection"/> instance with Hangfire services and the server registered.</returns>
        ///
        /// <author>Almighty-Shogun</author>
        /// <since>2.2.0</since>
        public IServiceCollection AddHangfire(CompatibilityLevel compatibilityLevel = CompatibilityLevel.Version_180)
        {
            serviceCollection.AddHangfire(config => config
                .SetDataCompatibilityLevel(compatibilityLevel)
                .UseSimpleAssemblyNameTypeSerializer()
                .UseRecommendedSerializerSettings()
                .UseInMemoryStorage());

            return serviceCollection.AddHangfireServer();
        }

        /// <summary>
        /// Registers recurring job classes discovered in the provided assemblies and schedules them when the host starts.
        /// </summary>
        ///
        /// <param name="assemblies">The assemblies to scan for recurring job classes. If none are provided, the calling assembly is used.</param>
        ///
        /// <returns>The <see cref="IServiceCollection"/> instance with recurring job implementations, job metadata, and the startup scheduler registered.</returns>
        ///
        /// <author>Almighty-Shogun</author>
        /// <since>2.2.0</since>
        public IServiceCollection RegisterRecurringJobs(params Assembly[] assemblies)
        {
            if (assemblies.Length == 0)
            {
                assemblies = [Assembly.GetCallingAssembly()];
            }

            serviceCollection.RegisterOnInherit<RecurringJobBase>(false, ServiceLifetime.Scoped, assemblies);

            foreach (RecurringJob job in HangfireUtils.GetRecurringJobs(assemblies))
            {
                serviceCollection.AddSingleton(job);
            }

            return serviceCollection.AddHostedService<JobSchedulerStartup>();
        }
    }
}
