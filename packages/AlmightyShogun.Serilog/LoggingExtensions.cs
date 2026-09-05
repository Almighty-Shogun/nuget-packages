using Serilog;
using Serilog.Core;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace AlmightyShogun.Serilog;

/// <summary>
/// Wires Serilog up with this package's color formatter, from either the service collection or the host builder. The two
/// entry points build the same logger configuration and register it differently: the service collection receiver adds a
/// Serilog provider alongside the ones already registered, while the host builder receiver hands the host's logging to
/// Serilog. Left to decide for itself, the package writes escape codes only when the process output is not redirected and
/// the <c>NO_COLOR</c> environment variable is unset or empty.
/// </summary>
///
/// <author>Almighty-Shogun</author>
/// <since>1.0.0</since>
public static class LoggingExtensions
{
    /// <summary>
    /// Provides the logging registration as an extension on the collection being built during startup.
    /// </summary>
    ///
    /// <param name="serviceCollection">
    /// The collection the logging provider is registered on. Use this receiver when the host builder is out of reach, such as
    /// from inside a registration module.
    /// </param>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>1.0.0</since>
    extension(IServiceCollection serviceCollection)
    {
        /// <summary>
        /// Registers Serilog as a logging provider through <c>AddLogging</c>, leaving any provider already registered on the
        /// collection in place. The color formatter reaches the logger only when <paramref name="includeConsoleSink"/> is
        /// <c>true</c>.
        /// </summary>
        ///
        /// <param name="configuration">Configuration supplying further Serilog settings, or nothing to skip that step.</param>
        /// <param name="includeConsoleSink">
        /// Whether this package attaches its own console sink, the one carrying the color formatter. Pass <c>false</c> when
        /// <paramref name="configuration"/> declares a console sink of its own, so a second one is not attached here.
        /// </param>
        /// <param name="enableColors">
        /// Whether escape codes are written, or nothing to take the default described on <see cref="LoggingExtensions"/>.
        /// </param>
        ///
        /// <returns>The <see cref="IServiceCollection"/> instance with logging configured.</returns>
        ///
        /// <remarks>
        /// A console provider registered elsewhere on the collection keeps writing unless it is removed. The logger is
        /// registered with <c>dispose: true</c>, which Serilog documents as disposing it when the framework disposes the
        /// provider.
        /// </remarks>
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
    /// Provides the logging registration as an extension on the host builder, for startup code that has the builder.
    /// </summary>
    ///
    /// <param name="hostBuilder">
    /// The generic host builder that takes over logging. Preferred over the service collection receiver wherever startup code
    /// has it in reach.
    /// </param>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>2.1.0</since>
    extension(IHostBuilder hostBuilder)
    {
        /// <summary>
        /// Hands the host's logging to Serilog through <c>UseSerilog</c>. The color formatter reaches the logger only when
        /// <paramref name="includeConsoleSink"/> is <c>true</c>.
        /// </summary>
        ///
        /// <param name="configuration">Configuration supplying further Serilog settings, or nothing to skip that step.</param>
        /// <param name="includeConsoleSink">
        /// Whether this package attaches its own console sink, the one carrying the color formatter. Pass <c>false</c> when
        /// <paramref name="configuration"/> declares a console sink of its own, so a second one is not attached here.
        /// </param>
        /// <param name="enableColors">
        /// Whether escape codes are written, or nothing to take the default described on <see cref="LoggingExtensions"/>.
        /// </param>
        ///
        /// <returns>The <see cref="IHostBuilder"/> instance with logging configured.</returns>
        ///
        /// <remarks>
        /// Serilog documents this overload as setting Serilog as the logging provider, with only Serilog sinks receiving
        /// events by default, so a provider the host registered does not write the line as well. The logger is registered
        /// with <c>dispose: true</c>, which Serilog documents as disposing it when the framework disposes the provider.
        /// </remarks>
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
    /// Builds a logger from the three settings, a distinct instance on every call, so an application that calls both
    /// receivers holds two, each with its own console sink.
    /// </summary>
    ///
    /// <param name="configuration">
    /// Configuration applied after the console sink, or nothing to skip that step. Its <c>Serilog</c> section is what supplies
    /// those settings, and the root object is also handed to any sink or enricher method that takes one.
    /// </param>
    /// <param name="includeConsoleSink">
    /// Whether to attach the console sink carrying the color formatter. With <c>false</c> and no configuration the logger is
    /// built with no sinks at all.
    /// </param>
    /// <param name="enableColors">
    /// Whether to write escape codes, or nothing to fall back to <see cref="ColorFormatter.OutputSupportsColors"/>. Read only
    /// when <paramref name="includeConsoleSink"/> is <c>true</c>.
    /// </param>
    ///
    /// <returns>A logger whose lifetime the caller owns.</returns>
    ///
    /// <remarks>
    /// The console sink, when one is attached, is wrapped in <c>WriteTo.Async</c>, so writes are queued on a background
    /// thread rather than blocking the caller. The queue is left at its default size with blocking off, which Serilog
    /// documents as dropping subsequent events once the queue is full.
    /// </remarks>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
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
