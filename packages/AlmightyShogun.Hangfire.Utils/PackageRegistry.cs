using Hangfire;
using System.Reflection;
using AlmightyShogun.Utils;
using Microsoft.Extensions.DependencyInjection;

namespace AlmightyShogun.Hangfire.Utils;

public static class PackageRegistry
{
    /// <param name="serviceCollection">The service collection used to register dependencies related to the Hangfire functionality.</param>
    extension(IServiceCollection serviceCollection)
    {
        /// <summary>
        /// Registers Hangfire and it's server with the specified <see cref="IServiceCollection"/>.
        /// </summary>
        /// <returns>The <see cref="IServiceCollection"/> instance with the Hangfire registered.</returns>
        /// 
        /// <author>Almighty-Shogun</author>
        /// <since>2.2.0</since>
        public IServiceCollection AddHangfire()
        {
            serviceCollection.AddHangfire(config => config
                .SetDataCompatibilityLevel(CompatibilityLevel.Version_180)
                .UseSimpleAssemblyNameTypeSerializer()
                .UseRecommendedSerializerSettings()
                .UseInMemoryStorage());
            
            return serviceCollection.AddHangfireServer();
        }

        /// <summary>
        /// Registers recurring job services for the specified assemblies using the provided <see cref="IServiceCollection"/>.
        /// </summary>
        /// 
        /// <param name="assemblies">An array of <see cref="Assembly"/> objects from which recurring jobs should be loaded from. If no assemblies are provided, the calling assembly is used by default.</param>
        ///
        /// <returns>The <see cref="IServiceCollection"/> instance with the recurring jobs registered.</returns>
        ///
        /// <author>Almighty-Shogun</author>
        /// <since>2.2.0</since>
        public IServiceCollection RegisterRecurringJobs(params Assembly[] assemblies)
        {
            if (assemblies.Length == 0)
            {
                assemblies = [Assembly.GetCallingAssembly()];
            }

            serviceCollection.RegisterOnInherit<IRecurringJob>(false, ServiceLifetime.Scoped, assemblies);

            foreach (RecurringJob job in HangfireUtils.GetRecurringJobs(assemblies))
            {
                serviceCollection.AddSingleton(job);
            }

            return serviceCollection;
        }
    }
}
