using Hangfire;
using System.Reflection;
using System.Runtime.CompilerServices;
using AlmightyShogun.Utils;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace AlmightyShogun.Hangfire.RecurringJobs;

/// <summary>
/// Provides registration for Hangfire and recurring jobs.
/// </summary>
///
/// <author>Almighty-Shogun</author>
/// <since>2.2.0</since>
public static class RecurringJobExtensions
{
    /// <summary>
    /// Provides recurring job registration extensions for a service collection.
    /// </summary>
    ///
    /// <param name="serviceCollection">
    /// The service collection.
    /// </param>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>2.2.0</since>
    extension(IServiceCollection serviceCollection)
    {
        /// <summary>
        /// Registers Hangfire with in-memory storage.
        /// </summary>
        ///
        /// <param name="addServer">Whether to register a Hangfire background processing server.</param>
        ///
        /// <returns>The service collection.</returns>
        ///
        /// <author>Almighty-Shogun</author>
        /// <since>2.2.0</since>
        public IServiceCollection AddCustomHangfire(bool addServer = true) => serviceCollection.AddCustomHangfire(
            static configuration => configuration.UseInMemoryStorage().SetDataCompatibilityLevel(CompatibilityLevel.Version_180),
            addServer);

        /// <summary>
        /// Registers Hangfire using the supplied configuration.
        /// </summary>
        ///
        /// <param name="configure">Configures Hangfire.</param>
        /// <param name="addServer">Whether to register a Hangfire background processing server.</param>
        ///
        /// <returns>The service collection.</returns>
        ///
        /// <author>Almighty-Shogun</author>
        /// <since>4.0.0</since>
        public IServiceCollection AddCustomHangfire(Action<IGlobalConfiguration> configure, bool addServer = true)
        {
            serviceCollection.AddHangfire(configuration =>
            {
                configuration
                    .UseSimpleAssemblyNameTypeSerializer()
                    .UseRecommendedSerializerSettings();

                configure(configuration);
            });

            return addServer ? serviceCollection.AddHangfireServer() : serviceCollection;
        }

        /// <summary>
        /// Registers recurring jobs from the calling assembly.
        /// </summary>
        ///
        /// <param name="configuration">Optional configuration containing recurring job overrides.</param>
        ///
        /// <returns>The service collection.</returns>
        ///
        /// <author>Almighty-Shogun</author>
        /// <since>2.2.0</since>
        [MethodImpl(MethodImplOptions.NoInlining)]
        public IServiceCollection RegisterRecurringJobs(IConfiguration? configuration = null)
            => serviceCollection.RegisterRecurringJobs([Assembly.GetCallingAssembly()], configuration);

        /// <summary>
        /// Registers recurring jobs from the specified assemblies.
        /// </summary>
        ///
        /// <param name="assemblies">The assemblies to scan for recurring jobs.</param>
        /// <param name="configuration">Optional configuration containing recurring job overrides.</param>
        ///
        /// <returns>The service collection.</returns>
        ///
        /// <author>Almighty-Shogun</author>
        /// <since>2.2.0</since>
        public IServiceCollection RegisterRecurringJobs(Assembly[] assemblies, IConfiguration? configuration = null)
        {
            foreach (Type jobType in TypeDiscovery.FindAssignableTypes<IRecurringJob>(assemblies))
                serviceCollection.AddScoped(jobType);

            serviceCollection.AddOptions<RecurringJobSettings>();

            if (configuration is not null)
                serviceCollection.AddConfiguration<RecurringJobSettings>(configuration.GetSection("RecurringJobs"));

            return serviceCollection
                .AddSingleton(new RecurringJobSources([.. assemblies]))
                .AddSingleton<RecurringJobRegistry>()
                .AddSingleton<IRecurringJobRegistry>(static provider => provider.GetRequiredService<RecurringJobRegistry>())
                .AddHostedService<JobSchedulerStartup>();
        }
    }
}
