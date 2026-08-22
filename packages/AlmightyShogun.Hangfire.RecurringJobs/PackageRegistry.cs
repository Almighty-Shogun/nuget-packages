using Hangfire;
using System.Reflection;
using AlmightyShogun.Core;
using Microsoft.Extensions.DependencyInjection;

namespace AlmightyShogun.Hangfire.RecurringJobs;

/// <summary>
/// Holds the two startup calls the package exposes: the Hangfire host setup, and the attribute scan that turns job classes
/// into schedules.
/// </summary>
///
/// <author>Almighty-Shogun</author>
/// <since>2.2.0</since>
public static class PackageRegistry
{
    /// <summary>
    /// Adds the package's two startup calls to the service collection. They are ordered: the scan schedules through the
    /// recurring job manager the Hangfire setup registers, so calling it alone leaves the host unable to resolve the scheduler.
    /// </summary>
    ///
    /// <param name="serviceCollection">
    /// The collection that receives the Hangfire services, the job classes, the singleton registry, and the hosted service
    /// that puts the schedules into Hangfire. Every helper returns it, so the calls chain.
    /// </param>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>2.2.0</since>
    extension(IServiceCollection serviceCollection)
    {
        /// <summary>
        /// Registers Hangfire with in-memory storage, and optionally the background processing server.
        /// </summary>
        ///
        /// <param name="compatibilityLevel">
        /// The storage and serialization format Hangfire writes. It only matters when this application shares storage with
        /// one running a different Hangfire version, which the in-memory store rules out.
        /// </param>
        /// <param name="addServer">
        /// Whether to run a background processing server in this application. Set it to <c>false</c> for a client that only
        /// enqueues work.
        /// </param>
        ///
        /// <returns>The <see cref="IServiceCollection"/> instance with Hangfire configured.</returns>
        ///
        /// <remarks>
        /// Storage is in-memory, so job state does not survive a restart and each instance keeps its own store. An
        /// application running more than one replica therefore runs every recurring job once per replica.
        /// </remarks>
        ///
        /// <author>Almighty-Shogun</author>
        /// <since>2.2.0</since>
        public IServiceCollection AddCustomHangfire(
            CompatibilityLevel compatibilityLevel = CompatibilityLevel.Version_180,
            bool addServer = true
        )
        {
            serviceCollection.AddHangfire(config => config
                .SetDataCompatibilityLevel(compatibilityLevel)
                .UseSimpleAssemblyNameTypeSerializer()
                .UseRecommendedSerializerSettings()
                .UseInMemoryStorage()
            );

            return addServer ? serviceCollection.AddHangfireServer() : serviceCollection;
        }

        /// <summary>
        /// Registers the recurring job classes declared by the calling assembly and schedules them when the host starts.
        /// </summary>
        ///
        /// <returns>The <see cref="IServiceCollection"/> instance with recurring jobs and the startup scheduler registered.</returns>
        ///
        /// <remarks>
        /// Reach for the <c>RegisterRecurringJobs(Assembly[])</c> overload when the jobs live somewhere other than the assembly
        /// making the call, which is the usual case once registration is factored into a shared startup extension.
        /// </remarks>
        ///
        /// <author>Almighty-Shogun</author>
        /// <since>2.2.0</since>
        public IServiceCollection RegisterRecurringJobs() => serviceCollection.RegisterRecurringJobs([Assembly.GetCallingAssembly()]);

        /// <summary>
        /// Registers the recurring job classes declared in the given assemblies and schedules them when the host starts.
        /// </summary>
        ///
        /// <param name="assemblies">
        /// The assemblies to scan for recurring job classes. Passing an empty array registers the scheduler with nothing to
        /// schedule rather than falling back to the calling assembly.
        /// </param>
        ///
        /// <returns>The <see cref="IServiceCollection"/> instance with recurring jobs and the startup scheduler registered.</returns>
        ///
        /// <remarks>
        /// The scan itself is deferred to the singleton registry, so an invalid cron expression, an unknown time zone, or a
        /// duplicate job id fails while the host starts rather than here. Job classes are registered scoped, so a job may
        /// depend on scoped services such as a database context; each run gets its own scope.
        /// </remarks>
        ///
        /// <author>Almighty-Shogun</author>
        /// <since>2.2.0</since>
        public IServiceCollection RegisterRecurringJobs(Assembly[] assemblies)
        {
            foreach (Type jobType in TypeDiscovery.FindAssignableTypes<RecurringJobBase>(assemblies))
                serviceCollection.AddScoped(jobType);

            return serviceCollection
                .AddSingleton(new RecurringJobSources(assemblies))
                .AddSingleton<IRecurringJobRegistry, RecurringJobRegistry>()
                .AddHostedService<JobSchedulerStartup>();
        }
    }
}
