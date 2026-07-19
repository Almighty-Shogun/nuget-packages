using AlmightyShogun.Utils;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace AlmightyShogun.AspNet.Maintenance;

/// <summary>
/// Registers maintenance mode services and middleware.
/// </summary>
///
/// <author>Almighty-Shogun</author>
/// <since>Unreleased</since>
public static class PackageRegistry
{
    /// <summary>
    /// Provides maintenance mode registration extension methods for the target service collection.
    /// </summary>
    ///
    /// <param name="serviceCollection">The service collection that receives the maintenance mode registrations.</param>
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
        /// <author>Almighty-Shogun</author>
        /// <since>Unreleased</since>
        public IServiceCollection AddMaintenanceMode(IConfiguration configuration) => serviceCollection
            .AddConfiguration<MaintenanceSettings>(configuration.GetSection("Maintenance"))
            .AddSingleton<IMaintenanceService, MaintenanceService>();
    }

    /// <summary>
    /// Provides maintenance mode middleware extension methods for the target application builder.
    /// </summary>
    ///
    /// <param name="applicationBuilder">The application builder that receives the maintenance mode middleware.</param>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    extension(IApplicationBuilder applicationBuilder)
    {
        /// <summary>
        /// Adds the maintenance mode middleware to the request pipeline.
        /// </summary>
        ///
        /// <returns>The <see cref="IApplicationBuilder"/> instance with maintenance mode middleware configured.</returns>
        ///
        /// <author>Almighty-Shogun</author>
        /// <since>Unreleased</since>
        public IApplicationBuilder UseMaintenanceMode()
            => applicationBuilder.UseMiddleware<MaintenanceMiddleware>();
    }
}
