using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Hosting.Internal;
using Microsoft.Extensions.DependencyInjection;

namespace AlmightyShogun.Hosting.Utils;

public static class ServiceCollectionExtensions
{
    extension(IServiceCollection serviceCollection)
    {
        /// <summary>
        /// Replaces the standard <see cref="ConsoleLifetime"/> with a custom implementation.
        /// The custom lifetime prevents the host application from shutting down when <c>Ctrl+C</c> is pressed outside an IDE.
        /// To allow shutdown via <c>Ctrl+C</c> in your IDE, set the environment variable
        /// <c>DOTNET_RUNNING_IN_IDE=true</c> in your run/debug configuration. This enables IDE stop commands
        /// or allows you to stop the application manually using <c>Ctrl+C</c>.
        /// </summary>
        /// 
        /// <returns>The updated <see cref="IServiceCollection"/> with the registered custom <see cref="ConsoleLifetime"/>.</returns>
        /// 
        /// <author>Almighty-Shogun</author>
        /// <since>2.0.0</since>
        public IServiceCollection UseCustomConsoleLifetime()
        {
            ServiceDescriptor? baseConsoleLifetime = serviceCollection
                .FirstOrDefault(sd => sd.ServiceType == typeof(IHostLifetime) && sd.ImplementationType == typeof(ConsoleLifetime));

            if (baseConsoleLifetime is not null)
            {
                serviceCollection.Remove(baseConsoleLifetime);
            }
    
            return serviceCollection.AddSingleton<IHostLifetime, CustomConsoleLifetime>();
        }

        /// <summary>
        /// Configures the host with the specified shutdown timeout and background service exception behavior.
        /// This method allows customization of host options, including handling of shutdown delays
        /// and configurable behavior when background services throw exceptions.
        /// </summary>
        /// 
        /// <param name="shutdownTimeout">The maximum amount of time to allow for a graceful host shutdown.</param>
        /// <param name="backgroundServiceExceptionBehavior">Specifies how the host should respond when a background service throws an exception.</param>
        /// 
        /// <returns>The updated <see cref="IServiceCollection"/> with the configured host options.</returns>
        ///
        /// <author>Almighty-Shogun</author>
        /// <since>2.0.0</since>
        public IServiceCollection ConfigureHost(TimeSpan shutdownTimeout, BackgroundServiceExceptionBehavior backgroundServiceExceptionBehavior)
        {
            return serviceCollection.Configure<HostOptions>(options =>
            {
                options.ShutdownTimeout = shutdownTimeout;
                options.BackgroundServiceExceptionBehavior = backgroundServiceExceptionBehavior;
            });
        }
    }
}
