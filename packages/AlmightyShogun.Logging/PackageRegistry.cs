using Serilog;
using Serilog.Core;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace AlmightyShogun.Logging;

/// <summary>
/// Registers the package Serilog configuration for service collections and host builders.
/// </summary>
///
/// <author>Almighty-Shogun</author>
/// <since>2.1.0</since>
public static class PackageRegistry
{
    /// <param name="serviceCollection">The service collection used to register the custom logging functionality.</param>
    extension(IServiceCollection serviceCollection)
    {
        /// <summary>
        /// Adds Serilog logging with the package color formatter to the service collection.
        /// </summary>
        ///
        /// <param name="configuration">Optional application configuration to load additional Serilog settings from.</param>
        ///
        /// <returns>The <see cref="IServiceCollection"/> instance with logging configured.</returns>
        ///
        /// <author>Almighty-Shogun</author>
        /// <since>1.0.0</since>
        public IServiceCollection AddCustomLogging(IConfiguration? configuration = null)
        {
            LoggerConfiguration loggerConfiguration = new LoggerConfiguration()
                .Enrich.FromLogContext()
                .WriteTo.Async(w => w.Console(new ColorFormatter()));

            if (configuration is not null)
                loggerConfiguration.ReadFrom.Configuration(configuration);

            Logger logger = loggerConfiguration.CreateLogger();

            return serviceCollection.AddLogging(builder => builder.AddSerilog(logger));
        }
    }

    /// <param name="hostBuilder">The hosting builder used to register the custom logging functionality.</param>
    extension(IHostBuilder hostBuilder)
    {
        /// <summary>
        /// Adds Serilog logging with the package color formatter to the host builder.
        /// </summary>
        ///
        /// <param name="configuration">Optional application configuration to load additional Serilog settings from.</param>
        ///
        /// <returns>The <see cref="IHostBuilder"/> instance with logging configured.</returns>
        ///
        /// <author>Almighty-Shogun</author>
        /// <since>2.1.0</since>
        public IHostBuilder AddCustomLogging(IConfiguration? configuration = null)
        {
            LoggerConfiguration loggerConfiguration = new LoggerConfiguration()
                .Enrich.FromLogContext()
                .WriteTo.Async(w => w.Console(new ColorFormatter()));

            if (configuration is not null)
                loggerConfiguration.ReadFrom.Configuration(configuration);

            Logger logger = loggerConfiguration.CreateLogger();

            return hostBuilder.UseSerilog(logger);
        }
    }
}
