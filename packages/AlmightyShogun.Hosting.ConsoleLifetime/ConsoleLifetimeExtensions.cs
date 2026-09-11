using AlmightyShogun.Utils;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;

namespace AlmightyShogun.Hosting.ConsoleLifetime;

/// <summary>
/// Provides console lifetime and host option configuration extensions.
/// </summary>
///
/// <author>Almighty-Shogun</author>
/// <since>2.0.0</since>
public static class ConsoleLifetimeExtensions
{
    /// <summary>
    /// Provides console lifetime extensions for <see cref="IServiceCollection"/>.
    /// </summary>
    ///
    /// <param name="serviceCollection">The service collection.</param>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>2.0.0</since>
    extension(IServiceCollection serviceCollection)
    {
        /// <summary>
        /// Replaces the host lifetime with the custom console lifetime.
        /// </summary>
        ///
        /// <returns>The configured service collection.</returns>
        ///
        /// <author>Almighty-Shogun</author>
        /// <since>2.0.0</since>
        public IServiceCollection UseCustomConsoleLifetime() => serviceCollection.ReplaceService<IHostLifetime, CustomConsoleLifetime>();

        /// <summary>
        /// Configures host shutdown behavior.
        /// </summary>
        ///
        /// <param name="shutdownTimeout">The shutdown timeout.</param>
        /// <param name="backgroundServiceExceptionBehavior">
        /// The background service exception behavior.
        /// </param>
        ///
        /// <returns>The configured service collection.</returns>
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
    /// Provides console lifetime extensions for <see cref="IHostApplicationBuilder"/>.
    /// </summary>
    ///
    /// <param name="hostApplicationBuilder">The host application builder.</param>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>4.0.0</since>
    extension(IHostApplicationBuilder hostApplicationBuilder)
    {
        /// <summary>
        /// Replaces the host lifetime with the custom console lifetime.
        /// </summary>
        ///
        /// <returns>The configured host application builder.</returns>
        ///
        /// <author>Almighty-Shogun</author>
        /// <since>4.0.0</since>
        public IHostApplicationBuilder UseCustomConsoleLifetime()
        {
            hostApplicationBuilder.Services.UseCustomConsoleLifetime();

            return hostApplicationBuilder;
        }

        /// <summary>
        /// Configures host shutdown behavior.
        /// </summary>
        ///
        /// <param name="shutdownTimeout">The shutdown timeout.</param>
        /// <param name="backgroundServiceExceptionBehavior">
        /// The background service exception behavior.
        /// </param>
        ///
        /// <returns>The configured host application builder.</returns>
        ///
        /// <author>Almighty-Shogun</author>
        /// <since>4.0.0</since>
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
    /// Provides console lifetime extensions for <see cref="IHostBuilder"/>.
    /// </summary>
    ///
    /// <param name="hostBuilder">The host builder.</param>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>4.0.0</since>
    extension(IHostBuilder hostBuilder)
    {
        /// <summary>
        /// Replaces the host lifetime with the custom console lifetime.
        /// </summary>
        ///
        /// <returns>The configured host builder.</returns>
        ///
        /// <author>Almighty-Shogun</author>
        /// <since>4.0.0</since>
        public IHostBuilder UseCustomConsoleLifetime()
            => hostBuilder.ConfigureServices(services => services.UseCustomConsoleLifetime());

        /// <summary>
        /// Configures host shutdown behavior.
        /// </summary>
        ///
        /// <param name="shutdownTimeout">The shutdown timeout.</param>
        /// <param name="backgroundServiceExceptionBehavior">
        /// The background service exception behavior.
        /// </param>
        ///
        /// <returns>The configured host builder.</returns>
        ///
        /// <author>Almighty-Shogun</author>
        /// <since>4.0.0</since>
        public IHostBuilder ConfigureHostOptions(
            TimeSpan shutdownTimeout,
            BackgroundServiceExceptionBehavior backgroundServiceExceptionBehavior
        ) => hostBuilder.ConfigureServices(services =>
        {
            services.ConfigureHostOptions(shutdownTimeout, backgroundServiceExceptionBehavior);
        });
    }
}
