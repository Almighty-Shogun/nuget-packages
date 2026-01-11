using Serilog;
using Serilog.Core;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace AlmightyShogun.Logging;

public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Adds logging functionality to the specified service collection using Serilog.
    /// </summary>
    /// 
    /// <param name="serviceCollection">The service collection to which logging services are added.</param>
    /// <param name="configuration">The<see cref="IConfiguration"/> to be provided if using Configuration</param>
    ///
    /// <returns>The <see cref="IServiceCollection"/> instance with logging configured.</returns>
    /// 
    /// <author>Almighty-Shogun</author>
    /// <since>1.0.0</since>
    public static IServiceCollection AddCustomLogging(this IServiceCollection serviceCollection, IConfiguration? configuration = null)
    {
        LoggerConfiguration loggerConfiguration = new LoggerConfiguration()
            .Enrich.FromLogContext()
            .WriteTo.Async(loggerConfiguration => loggerConfiguration.Console(new ColorFormatter()));

        if (configuration is not null)
            loggerConfiguration.ReadFrom.Configuration(configuration);

        Logger logger = loggerConfiguration.CreateLogger();

        return serviceCollection.AddLogging(builder => builder.AddSerilog(logger));
    }
}
