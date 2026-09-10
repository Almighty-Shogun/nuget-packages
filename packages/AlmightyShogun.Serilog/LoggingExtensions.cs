using Serilog;
using Serilog.Core;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace AlmightyShogun.Serilog;

/// <summary>
/// Provides extensions for configuring Serilog with this package's log formatting.
/// </summary>
///
/// <author>Almighty-Shogun</author>
/// <since>1.0.0</since>
public static class LoggingExtensions
{
    /// <summary>
    /// Provides Serilog logging configuration for an <see cref="IServiceCollection"/>.
    /// </summary>
    ///
    /// <param name="serviceCollection">
    /// The service collection to configure.
    /// </param>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>1.0.0</since>
    extension(IServiceCollection serviceCollection)
    {
        /// <summary>
        /// Adds Serilog logging with optional console output and configuration.
        /// </summary>
        ///
        /// <param name="configuration">The Serilog configuration to apply, or <c>null</c> to skip configuration-based settings.</param>
        /// <param name="includeConsoleSink">
        /// Whether to add the package's console sink.
        /// </param>
        /// <param name="enableColors">
        /// Whether to enable ANSI colors, or <c>null</c> to detect color support automatically.
        /// </param>
        ///
        /// <returns>The configured service collection.</returns>
        ///
        /// <author>Almighty-Shogun</author>
        /// <since>1.0.0</since>
        public IServiceCollection AddCustomLogging(
            IConfiguration? configuration = null,
            bool includeConsoleSink = true,
            bool? enableColors = null
        )
        {
            Logger logger = CreateLogger(configuration, includeConsoleSink, enableColors);

            return serviceCollection.AddLogging(builder => builder.AddSerilog(logger, true));
        }
    }

    /// <summary>
    /// Provides Serilog logging configuration for an <see cref="IHostBuilder"/>.
    /// </summary>
    ///
    /// <param name="hostBuilder">
    /// The host builder to configure.
    /// </param>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>2.1.0</since>
    extension(IHostBuilder hostBuilder)
    {
        /// <summary>
        /// Configures the host to use Serilog with optional console output and configuration.
        /// </summary>
        ///
        /// <param name="configuration">The Serilog configuration to apply, or <c>null</c> to skip configuration-based settings.</param>
        /// <param name="includeConsoleSink">
        /// Whether to add the package's console sink.
        /// </param>
        /// <param name="enableColors">
        ///  Whether to enable ANSI colors, or <c>null</c> to detect color support automatically.
        /// </param>
        ///
        /// <returns>The configured <see cref="IHostBuilder"/>.</returns>
        ///
        /// <author>Almighty-Shogun</author>
        /// <since>2.1.0</since>
        public IHostBuilder AddCustomLogging(
            IConfiguration? configuration = null,
            bool includeConsoleSink = true,
            bool? enableColors = null
        )
        {
            Logger logger = CreateLogger(configuration, includeConsoleSink, enableColors);

            return hostBuilder.UseSerilog(logger, true);
        }
    }

    /// <summary>
    /// Creates a Serilog logger with optional console output and configuration.
    /// </summary>
    ///
    /// <param name="configuration">
    /// The Serilog configuration to apply, or <c>null</c> to skip configuration-based settings.
    /// </param>
    /// <param name="includeConsoleSink">
    /// Whether to add the package's console sink.
    /// </param>
    /// <param name="enableColors">
    /// Whether to enable ANSI colors, or <c>null</c> to detect color support automatically.
    /// </param>
    ///
    /// <returns>A configured logger instance.</returns>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>4.0.0</since>
    private static Logger CreateLogger(IConfiguration? configuration, bool includeConsoleSink, bool? enableColors)
    {
        LoggerConfiguration loggerConfiguration = new LoggerConfiguration().Enrich.FromLogContext();

        if (includeConsoleSink)
        {
            bool colors = enableColors ?? ColorFormatter.OutputSupportsColors;

            loggerConfiguration.WriteTo.Async(w => w.Console(new ColorFormatter(colors)));
        }

        if (configuration is not null)
            loggerConfiguration.ReadFrom.Configuration(configuration);

        return loggerConfiguration.CreateLogger();
    }
}
