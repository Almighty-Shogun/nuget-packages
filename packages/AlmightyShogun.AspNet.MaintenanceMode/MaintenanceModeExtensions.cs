using AlmightyShogun.Utils;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace AlmightyShogun.AspNet.MaintenanceMode;

/// <summary>
/// Registers the maintenance services and the middleware as two separate calls, so an application can own the state without blocking any
/// request.
/// </summary>
///
/// <author>Almighty-Shogun</author>
/// <since>Unreleased</since>
public static class MaintenanceModeExtensions
{
    /// <summary>
    /// Provides the registration helper as an extension on the collection. It registers this package's own services and nothing else, so
    /// the <c>IHttpErrorResponseWriter</c> the middleware writes through has to be added separately.
    /// </summary>
    ///
    /// <param name="serviceCollection">
    /// The collection that receives the registrations. The helper returns it, so calls can be chained or written as separate statements
    /// without difference.
    /// </param>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    extension(IServiceCollection serviceCollection)
    {
        /// <summary>
        /// Registers maintenance mode options and the file-backed service implementation.
        /// </summary>
        ///
        /// <param name="configuration">The application configuration that may contain a <c>Maintenance</c> section.</param>
        ///
        /// <returns>The <see cref="IServiceCollection"/> instance with maintenance mode services registered.</returns>
        ///
        /// <remarks>
        /// Requires <c>AddHttpErrorResponseWriter</c> from <c>AlmightyShogun.AspNet.Core</c>, which is what the middleware writes the
        /// blocked-request body through. Nothing else is needed: the body carries the window's own message rather than a resolved one.
        /// </remarks>
        ///
        /// <remarks>
        /// The service is registered under its own type as well as under <see cref="IMaintenanceService"/>, resolving to one instance
        /// either way. The middleware takes the concrete type because it reads the persisted window, which is internal and deliberately
        /// absent from the public interface; without that registration <c>UseMaintenanceMode</c> cannot activate the middleware at all.
        /// </remarks>
        ///
        /// <author>Almighty-Shogun</author>
        /// <since>Unreleased</since>
        public IServiceCollection AddMaintenanceMode(IConfiguration configuration) => serviceCollection
            .AddConfiguration<MaintenanceSettings>(configuration.GetSection("Maintenance"))
            .AddSingleton<IMaintenanceStore, FileMaintenanceStore>()
            .AddSingleton<MaintenanceService>()
            .AddSingleton<IMaintenanceService>(provider => provider.GetRequiredService<MaintenanceService>());
    }

    /// <summary>
    /// Provides the pipeline helper as an extension on the builder. It appends middleware, so where it is called decides which requests it
    /// covers, and it registers none of the services that middleware resolves.
    /// </summary>
    ///
    /// <param name="applicationBuilder">
    /// The pipeline the middleware is appended to. Order matters, so this belongs at the point in <c>Program.cs</c> where the middleware
    /// should sit rather than grouped with other registrations for tidiness.
    /// </param>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    extension(IApplicationBuilder applicationBuilder)
    {
        /// <summary>
        /// Adds the middleware that blocks requests while a window is open. Place it early, ahead of routing and authentication, so a
        /// blocked request is answered before anything else runs, but after <c>UseForwardedHeaders</c>.
        /// </summary>
        ///
        /// <returns>The <see cref="IApplicationBuilder"/> instance with maintenance mode middleware configured.</returns>
        ///
        /// <remarks>
        /// The address bypass reads the connection address, which behind a reverse proxy is the proxy until
        /// <c>UseForwardedHeaders</c> has run. Calling this before it would compare an allow list against the proxy's address rather than
        /// the caller's, so run <c>UseForwardedHeaders</c> first and configure its trusted proxies and networks.
        /// </remarks>
        ///
        /// <author>Almighty-Shogun</author>
        /// <since>Unreleased</since>
        public IApplicationBuilder UseMaintenanceMode() => applicationBuilder.UseMiddleware<MaintenanceMiddleware>();
    }
}
