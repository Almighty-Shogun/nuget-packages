using AlmightyShogun.Utils;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace AlmightyShogun.AspNet.MaintenanceMode;

/// <summary>
/// Provides extensions for configuring maintenance mode.
/// </summary>
///
/// <author>Almighty-Shogun</author>
/// <since>4.0.0</since>
public static class MaintenanceModeExtensions
{
    /// <summary>
    /// Registers maintenance mode services.        
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>4.0.0</since>
    extension(IServiceCollection serviceCollection)
    {
        /// <summary>
        /// Registers maintenance mode services.
        /// </summary>
        ///
        /// <param name="configuration">The application configuration.</param>
        ///
        /// <returns>The service collection.</returns>
        ///
        /// <remarks>
        /// Requires the HTTP error response writer from <c>AlmightyShogun.AspNet.Core</c>.
        /// </remarks>
        ///
        /// <author>Almighty-Shogun</author>
        /// <since>4.0.0</since>
        public IServiceCollection AddMaintenanceMode(IConfiguration configuration) => serviceCollection
            .AddConfiguration<MaintenanceSettings>(configuration.GetSection("Maintenance"))
            .AddSingleton<IMaintenanceStore, FileMaintenanceStore>()
            .AddSingleton<MaintenanceService>()
            .AddSingleton<IMaintenanceService>(provider => provider.GetRequiredService<MaintenanceService>());
    }

    /// <summary>
    /// Provides maintenance mode middleware extensions.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>4.0.0</since>
    extension(IApplicationBuilder applicationBuilder)
    {
        /// <summary>
        /// Adds maintenance mode middleware to the request pipeline.
        /// </summary>
        ///
        /// <returns>The application builder.</returns>
        ///
        /// <remarks>
        /// Run forwarded headers and path base middleware first when they are used.
        /// </remarks>
        ///
        /// <author>Almighty-Shogun</author>
        /// <since>4.0.0</since>
        public IApplicationBuilder UseMaintenanceMode() => applicationBuilder.UseMiddleware<MaintenanceMiddleware>();
    }
}
