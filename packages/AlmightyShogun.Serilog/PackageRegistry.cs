using Serilog;
using Serilog.Core;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace AlmightyShogun.Serilog;

/// <summary>
/// Wires Serilog up with this package's color formatter, from either the service collection or the host builder. The two
/// entry points build the same logger; which one to use depends only on what startup code has in hand.
/// </summary>
///
/// <author>Almighty-Shogun</author>
/// <since>2.1.0</since>
public static class PackageRegistry
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
        /// Registers Serilog with the color formatter as a logging provider, leaving the host's existing providers in place.
        /// </summary>
        ///
        /// <param name="configuration">
        /// Configuration read for further Serilog settings, applied after the console sink so it can add sinks, override the
        /// minimum level, or attach enrichers. Pass nothing to run on the formatter alone.
        /// </param>
        /// <param name="includeConsoleSink">
        /// Whether this package attaches its own console sink. Pass <c>false</c> when configuration already declares one,
        /// otherwise every line is written twice.
        /// </param>
        /// <param name="enableColors">
        /// Whether escape codes are written. Left unset, it follows whether the output looks like a terminal, which is the
        /// right answer for a process that is sometimes piped and sometimes not.
        /// </param>
        ///
        /// <returns>The <see cref="IServiceCollection"/> instance with logging configured.</returns>
        ///
        /// <remarks>
        /// This adds a Serilog provider next to whatever logging the host already registered, so the host's own console
        /// provider keeps writing unless it is removed. The <see cref="IHostBuilder"/> receiver takes over instead.
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
    /// The generic host builder that takes over logging. Preferred over the service collection receiver, because it replaces
    /// the host's own providers rather than adding one alongside them.
    /// </param>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>2.1.0</since>
    extension(IHostBuilder hostBuilder)
    {
        /// <summary>
        /// Hands the host's logging to Serilog with the color formatter, replacing the providers it would otherwise use.
        /// </summary>
        ///
        /// <param name="configuration">
        /// Configuration read for further Serilog settings, applied after the console sink so it can add sinks, override the
        /// minimum level, or attach enrichers. Pass nothing to run on the formatter alone.
        /// </param>
        /// <param name="includeConsoleSink">
        /// Whether this package attaches its own console sink. Pass <c>false</c> when configuration already declares one,
        /// otherwise every line is written twice.
        /// </param>
        /// <param name="enableColors">
        /// Whether escape codes are written. Left unset, it follows whether the output looks like a terminal, which is the
        /// right answer for a process that is sometimes piped and sometimes not.
        /// </param>
        ///
        /// <returns>The <see cref="IHostBuilder"/> instance with logging configured.</returns>
        ///
        /// <remarks>
        /// This replaces the host's logging providers rather than adding to them, so a line is written once even when the
        /// default console provider was already in place.
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
    /// Builds the logger both receivers share, so they cannot drift apart in how they configure it.
    /// </summary>
    ///
    /// <param name="configuration">Configuration applied after the console sink, or nothing to skip that step.</param>
    /// <param name="includeConsoleSink">Whether to attach the console sink carrying the color formatter.</param>
    /// <param name="enableColors">
    /// Whether to write escape codes, or nothing to fall back to <see cref="ColorFormatter.OutputSupportsColors"/>.
    /// </param>
    ///
    /// <returns>
    /// A logger the caller must arrange to dispose. Both receivers hand ownership to Serilog so it is closed with the host.
    /// </returns>
    ///
    /// <remarks>
    /// The console sink is wrapped in <c>WriteTo.Async</c>, so writes are queued on a background thread rather than blocking
    /// the caller. Buffered entries are lost if the process exits without the logger being disposed.
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
