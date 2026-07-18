using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace AlmightyShogun.AspNet.Utils;

/// <summary>
/// Registers the ASP.NET Core helpers provided by the package.
/// </summary>
///
/// <author>Almighty-Shogun</author>
/// <since>2.2.1</since>
public static class PackageRegistry
{
    /// <param name="serviceCollection">The service collection used to register the ASP.NET functionality.</param>
    extension(IServiceCollection serviceCollection)
    {
        /// <summary>
        /// Registers MVC controller services and adds the package action filters globally.
        /// </summary>
        ///
        /// <returns>The <see cref="IServiceCollection"/> instance with controller action filters configured.</returns>
        ///
        /// <author>Almighty-Shogun</author>
        /// <since>2.2.1</since>
        public IServiceCollection AddActionFilters()
        {
            serviceCollection.AddControllers(options => options.Filters.Add<SessionContextFilter>());

            return serviceCollection;
        }

        /// <summary>
        /// Registers a named CORS policy using origins from the <c>AllowedOrigins</c> configuration section.
        /// </summary>
        ///
        /// <param name="name">The name of the CORS policy to register.</param>
        /// <param name="configuration">The application configuration that contains the <c>AllowedOrigins</c> string array.</param>
        ///
        /// <returns>The <see cref="IServiceCollection"/> instance with allowed origins configured.</returns>
        ///
        /// <author>Almighty-Shogun</author>
        /// <since>2.2.1</since>
        public IServiceCollection AddAllowedOrigins(string name, IConfiguration configuration) => serviceCollection.AddCors(options =>
        {
            string[] allowedOrigins = configuration.GetSection("AllowedOrigins").Get<string[]>() ?? [];

            options.AddPolicy(name, policy => policy
                .WithOrigins(allowedOrigins)
                .AllowAnyHeader()
                .AllowAnyMethod()
                .AllowCredentials());
        });
    }
}
