using Serilog;
using Serilog.Core;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace AlmightyShogun.Logging;

public static class PackageRegistry
{
    /// <param name="serviceCollection">The service collection used to register the custom logging functionality.</param>
    extension(IServiceCollection serviceCollection)
    {
        /// <summary>
        /// Adds logging functionality to the specified service collection using Serilog.
        /// </summary>
        /// 
        /// <param name="configuration">The<see cref="IConfiguration"/> to be provided if using Configuration</param>
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
        /// Adds logging functionality to the specified hosting builder using Serilog.
        /// </summary>
        /// 
        /// <param name="configuration">The<see cref="IConfiguration"/> to be provided if using Configuration</param>
        ///
        /// <returns>The <see cref="IServiceCollection"/> instance with logging configured.</returns>
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
