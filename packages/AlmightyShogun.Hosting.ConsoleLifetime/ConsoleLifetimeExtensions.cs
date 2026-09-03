using AlmightyShogun.Utils;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;

namespace AlmightyShogun.Hosting.ConsoleLifetime;

/// <summary>
/// Provides the two startup helpers this package contributes: taking over the console lifetime so <c>Ctrl+C</c> no longer
/// stops the process, and setting the host options that govern shutdown. Each is offered on all three startup entry points.
/// </summary>
///
/// <author>Almighty-Shogun</author>
/// <since>2.0.0</since>
public static class ConsoleLifetimeExtensions
{
    /// <summary>
    /// Provides both helpers on the service collection, which is where the registrations actually land. The two builder
    /// receivers below forward here rather than registering anything of their own.
    /// </summary>
    ///
    /// <param name="serviceCollection">
    /// The collection the registrations are made on. Use this receiver from a registration module or anywhere the builder
    /// itself is out of reach. Both helpers return it so calls can be chained.
    /// </param>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>2.0.0</since>
    extension(IServiceCollection serviceCollection)
    {
        /// <summary>
        /// Takes over the console lifetime so <c>Ctrl+C</c> no longer stops the application, for a worker or daemon that
        /// should only stop when something asks it to. <c>SIGTERM</c> still shuts the host down in an orderly way.
        /// </summary>
        ///
        /// <returns>The <see cref="IServiceCollection"/> instance with the custom <see cref="IHostLifetime"/> registered.</returns>
        ///
        /// <remarks>
        /// Set <c>DOTNET_RUNNING_IN_IDE</c> in a run configuration to keep <c>Ctrl+C</c> working while debugging, otherwise a
        /// locally launched process can only be stopped from outside.
        ///
        /// The default lifetime is replaced rather than added, so calling this twice still leaves exactly one registration.
        /// </remarks>
        ///
        /// <author>Almighty-Shogun</author>
        /// <since>2.0.0</since>
        public IServiceCollection UseCustomConsoleLifetime() => serviceCollection.ReplaceService<IHostLifetime, CustomConsoleLifetime>();

        /// <summary>
        /// Sets how long shutdown may take and what a failing background service does to the host.
        /// </summary>
        ///
        /// <param name="shutdownTimeout">
        /// How long the host waits for hosted services to stop before it gives up and continues shutting down. Too short
        /// truncates work that was mid-flight; too long delays a restart and can trip an orchestrator's own kill timeout.
        /// </param>
        /// <param name="backgroundServiceExceptionBehavior">
        /// What an unhandled exception in a <see cref="BackgroundService"/> does.
        /// <see cref="BackgroundServiceExceptionBehavior.StopHost"/> brings the whole application down, which surfaces the
        /// fault; <see cref="BackgroundServiceExceptionBehavior.Ignore"/> logs it and leaves the process running with that one
        /// service dead.
        /// </param>
        ///
        /// <returns>The <see cref="IServiceCollection"/> instance with the host options configured.</returns>
        ///
        /// <author>Almighty-Shogun</author>
        /// <since>2.0.0</since>
        public IServiceCollection ConfigureHostOptions(
            TimeSpan shutdownTimeout,
            BackgroundServiceExceptionBehavior backgroundServiceExceptionBehavior
        ) => serviceCollection.Configure<HostOptions>(options =>
        {
            options.ShutdownTimeout = shutdownTimeout;
            options.BackgroundServiceExceptionBehavior = backgroundServiceExceptionBehavior;
        });
    }

    /// <summary>
    /// Provides both helpers on the modern host builder, each forwarding to the service collection receiver so startup code
    /// never has to reach through <c>Services</c> itself.
    /// </summary>
    ///
    /// <param name="hostApplicationBuilder">
    /// The builder whose services receive the registrations. This is the receiver to reach for in a modern minimal-hosting
    /// <c>Program.cs</c>, where the builder is what startup code has in hand.
    /// </param>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    extension(IHostApplicationBuilder hostApplicationBuilder)
    {
        /// <summary>
        /// Takes over the console lifetime so <c>Ctrl+C</c> no longer stops the application, leaving <c>SIGTERM</c> as the
        /// orderly way out.
        /// </summary>
        ///
        /// <returns>The <see cref="IHostApplicationBuilder"/> instance with the custom <see cref="IHostLifetime"/> registered.</returns>
        ///
        /// <remarks>
        /// Delegates to the <see cref="IServiceCollection"/> receiver, so the behavior and the <c>DOTNET_RUNNING_IN_IDE</c>
        /// escape hatch documented there apply unchanged.
        /// </remarks>
        ///
        /// <author>Almighty-Shogun</author>
        /// <since>Unreleased</since>
        public IHostApplicationBuilder UseCustomConsoleLifetime()
        {
            hostApplicationBuilder.Services.UseCustomConsoleLifetime();

            return hostApplicationBuilder;
        }

        /// <summary>
        /// Sets how long shutdown may take and what a failing background service does to the host.
        /// </summary>
        ///
        /// <param name="shutdownTimeout">
        /// How long the host waits for hosted services to stop before it gives up and continues shutting down.
        /// </param>
        /// <param name="backgroundServiceExceptionBehavior">
        /// Whether an unhandled exception in a <see cref="BackgroundService"/> stops the host or is logged and ignored.
        /// </param>
        ///
        /// <returns>The <see cref="IHostApplicationBuilder"/> instance with the host options configured.</returns>
        ///
        /// <author>Almighty-Shogun</author>
        /// <since>Unreleased</since>
        public IHostApplicationBuilder ConfigureHostOptions(
            TimeSpan shutdownTimeout,
            BackgroundServiceExceptionBehavior backgroundServiceExceptionBehavior
        )
        {
            hostApplicationBuilder.Services.ConfigureHostOptions(shutdownTimeout, backgroundServiceExceptionBehavior);

            return hostApplicationBuilder;
        }
    }

    /// <summary>
    /// Provides both helpers on the generic host builder, each deferring its registration into
    /// <see cref="IHostBuilder.ConfigureServices"/> rather than applying it where it is called.
    /// </summary>
    ///
    /// <param name="hostBuilder">
    /// The generic host builder that receives the registrations. This is the receiver for an application still built with
    /// <c>Host.CreateDefaultBuilder</c> rather than the newer builder.
    /// </param>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    extension(IHostBuilder hostBuilder)
    {
        /// <summary>
        /// Takes over the console lifetime so <c>Ctrl+C</c> no longer stops the application, leaving <c>SIGTERM</c> as the
        /// orderly way out.
        /// </summary>
        ///
        /// <returns>The <see cref="IHostBuilder"/> instance with the custom <see cref="IHostLifetime"/> registered.</returns>
        ///
        /// <remarks>
        /// Registration is deferred into <see cref="IHostBuilder.ConfigureServices"/>, so it lands whenever the builder runs
        /// its callbacks rather than at the moment this is called.
        /// </remarks>
        ///
        /// <author>Almighty-Shogun</author>
        /// <since>Unreleased</since>
        public IHostBuilder UseCustomConsoleLifetime()
            => hostBuilder.ConfigureServices(services => services.UseCustomConsoleLifetime());

        /// <summary>
        /// Sets how long shutdown may take and what a failing background service does to the host.
        /// </summary>
        ///
        /// <param name="shutdownTimeout">
        /// How long the host waits for hosted services to stop before it gives up and continues shutting down.
        /// </param>
        /// <param name="backgroundServiceExceptionBehavior">
        /// Whether an unhandled exception in a <see cref="BackgroundService"/> stops the host or is logged and ignored.
        /// </param>
        ///
        /// <returns>The <see cref="IHostBuilder"/> instance with the host options configured.</returns>
        ///
        /// <author>Almighty-Shogun</author>
        /// <since>Unreleased</since>
        public IHostBuilder ConfigureHostOptions(
            TimeSpan shutdownTimeout,
            BackgroundServiceExceptionBehavior backgroundServiceExceptionBehavior
        ) => hostBuilder.ConfigureServices(services =>
        {
            services.ConfigureHostOptions(shutdownTimeout, backgroundServiceExceptionBehavior);
        });
    }
}
