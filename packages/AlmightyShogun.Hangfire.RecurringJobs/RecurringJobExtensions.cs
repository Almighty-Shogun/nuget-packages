using Hangfire;
using System.Reflection;
using AlmightyShogun.Utils;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace AlmightyShogun.Hangfire.RecurringJobs;

/// <summary>
/// Holds the two startup calls the package exposes: the Hangfire host setup, and the attribute scan that turns job classes
/// into schedules.
/// </summary>
///
/// <author>Almighty-Shogun</author>
/// <since>2.2.0</since>
public static class RecurringJobExtensions
{
    /// <summary>
    /// Adds the package's two startup calls to the service collection. Neither resolves anything, so they may be written in
    /// either order, but both are needed: the scan only records what to schedule, and the hosted service it registers hands
    /// those schedules to the recurring job manager the Hangfire setup provides.
    /// </summary>
    ///
    /// <param name="serviceCollection">
    /// The collection that receives the Hangfire services, the job classes, the bound settings, the singleton registry, and
    /// the hosted service that puts the schedules into Hangfire. Every helper returns it, so the calls chain.
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
        /// <param name="addServer">
        /// Whether to run a background processing server in this application. Set it to <c>false</c> for a client that only
        /// enqueues work.
        /// </param>
        ///
        /// <returns>The <see cref="IServiceCollection"/> instance with Hangfire configured.</returns>
        ///
        /// <remarks>
        /// In-memory storage loses job state on restart and gives every replica its own store, so an application running more
        /// than one replica runs each recurring job once per replica. Reach for the delegate overload to point Hangfire at a
        /// durable store instead.
        /// </remarks>
        ///
        /// <author>Almighty-Shogun</author>
        /// <since>2.2.0</since>
        public IServiceCollection AddCustomHangfire(bool addServer = true) => serviceCollection.AddCustomHangfire(
            static configuration => configuration.UseInMemoryStorage().SetDataCompatibilityLevel(CompatibilityLevel.Version_180),
            addServer
        );

        /// <summary>
        /// Registers Hangfire with serializer defaults, the caller's storage and data compatibility configuration, and
        /// optionally the background processing server.
        /// </summary>
        ///
        /// <param name="configure">
        /// Selects the storage provider and data compatibility level. Hangfire throws when no storage is set by the final
        /// configuration; the compatibility level has a default and may be left alone.
        /// </param>
        /// <param name="addServer">
        /// Whether to run a background processing server in this application. Set it to <c>false</c> for a client that only
        /// enqueues work.
        /// </param>
        ///
        /// <returns>The <see cref="IServiceCollection"/> instance with Hangfire configured.</returns>
        ///
        /// <remarks>
        /// The package still applies the simple assembly-name type serializer and recommended serializer settings, but the
        /// delegate owns the storage and data compatibility level. An application sharing a store with one running a
        /// different Hangfire version calls <c>SetDataCompatibilityLevel</c> to match it, since a newer level writes payloads
        /// an older reader cannot deserialize.
        /// </remarks>
        ///
        /// <author>Almighty-Shogun</author>
        /// <since>Unreleased</since>
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
        /// Registers the recurring job classes declared by the calling assembly and schedules them when the host starts.
        /// </summary>
        ///
        /// <param name="configuration">
        /// Read for a <c>RecurringJobs</c> section overriding what the attributes declare. Leave it out when every
        /// environment schedules the same jobs the same way.
        /// </param>
        ///
        /// <returns>The <see cref="IServiceCollection"/> instance with recurring jobs and the startup scheduler registered.</returns>
        ///
        /// <remarks>
        /// Reach for the <c>RegisterRecurringJobs(Assembly[], IConfiguration)</c> overload when the jobs live somewhere other
        /// than the assembly making the call, which is the usual case once registration is factored into a shared startup
        /// extension.
        /// </remarks>
        ///
        /// <author>Almighty-Shogun</author>
        /// <since>2.2.0</since>
        public IServiceCollection RegisterRecurringJobs(IConfiguration? configuration = null)
            => serviceCollection.RegisterRecurringJobs([Assembly.GetCallingAssembly()], configuration);

        /// <summary>
        /// Registers the recurring job classes declared in the given assemblies and schedules them when the host starts.
        /// </summary>
        ///
        /// <param name="assemblies">
        /// The assemblies to scan for recurring job classes. Passing an empty array registers the scheduler with nothing to
        /// schedule rather than falling back to the calling assembly.
        /// </param>
        /// <param name="configuration">
        /// Read for a <c>RecurringJobs</c> section overriding what the attributes declare. Leave it out when every
        /// environment schedules the same jobs the same way.
        /// </param>
        ///
        /// <returns>The <see cref="IServiceCollection"/> instance with recurring jobs and the startup scheduler registered.</returns>
        ///
        /// <remarks>
        /// The scan itself is deferred to the singleton registry, so an invalid cron expression, an unknown time zone, a
        /// duplicate job id, or an override naming a job nothing declares fails while the host starts rather than here. Job
        /// classes are registered scoped, so a job may depend on scoped services such as a database context; each run gets
        /// its own scope.
        /// </remarks>
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
                .AddSingleton<IRecurringJobRegistry, RecurringJobRegistry>()
                .AddHostedService<JobSchedulerStartup>();
        }
    }
}
