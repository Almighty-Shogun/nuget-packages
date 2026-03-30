using AlmightyShogun.AspNet.Utils.Filters;

namespace AlmightyShogun.AspNet.Utils;

public static class PackageRegistry
{
    /// <param name="serviceCollection">The service collection used to register the ASP.NET functionality.</param>
    extension(IServiceCollection serviceCollection)
    {
        /// <summary>
        /// Registers MVC controller services and configures the global action filters
        /// </summary>
        ///
        /// <returns>The <see cref="IServiceCollection"/> instance with allowed origins configured.</returns>
        ///
        /// <author>Almighty-Shogun</author>
        /// <since>2.2.0</since>
        public IServiceCollection AddActionFilters()
        {
            serviceCollection.AddControllers(options =>
            {
                options.Filters.Add<SessionContextFilter>();
            });
            
            return serviceCollection;
        }
        
        /// <summary>
        /// 
        /// </summary>
        
        /// <param name="name">The name of the allowed origins' policy.</param>
        /// <param name="configuration">The<see cref="IConfiguration"/> to be provided.</param>
        ///
        /// <returns>The <see cref="IServiceCollection"/> instance with allowed origins configured.</returns>
        ///
        /// <author>Almighty-Shogun</author>
        /// <since>1.0.0</since>
        public IServiceCollection AddAllowedOrigins(string name, IConfiguration configuration) => serviceCollection.AddCors(options =>
        {
            options.AddPolicy(name, policy =>
            {
                policy.WithOrigins(configuration.GetSection("AllowedOrigins").Get<string[]>() ?? [])
                    .AllowAnyHeader()
                    .AllowAnyMethod()
                    .AllowCredentials();
            });
        });
    }
}
