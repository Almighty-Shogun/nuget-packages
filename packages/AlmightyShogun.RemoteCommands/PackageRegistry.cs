using System.Reflection;
using AlmightyShogun.Utils;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace AlmightyShogun.RemoteCommands;

public static class PackageRegistry
{
    /// <param name="serviceCollection">The service collection used to register dependencies related to remote command functionality.</param>
    extension(IServiceCollection serviceCollection)
    {
        /// <summary>
        /// Registers remote command services with the specified <see cref="IServiceCollection"/>.
        /// </summary>
        /// 
        /// <param name="configuration">The<see cref="IConfiguration"/> to be provided if using Configuration</param>
        /// 
        /// <returns>The <see cref="IServiceCollection"/> instance with the remote commands handlers registered.</returns>
        /// 
        /// <author>Almighty-Shogun</author>
        /// <since>1.0.0</since>
        public IServiceCollection AddRemoteCommands(IConfiguration configuration)
        {
            var settings = configuration.GetSection("RemoteServer").Get<RemoteServerSettings>();
            
            if (settings is null)
                throw new InvalidOperationException("Missing RemoteServer configuration");
            
            return serviceCollection
                .AddConfiguration<RemoteServerSettings>(configuration.GetSection("RemoteServer"))
                .AddSingleton<IRemoteCommandHandler, RemoteCommandHandler>();
        }

        /// <summary>
        /// Registers implementations of <see cref="IRemoteCommand"/> in the specified assemblies with the DI container.
        /// </summary>
        /// 
        /// <param name="assemblies">The assemblies to scan for types implementing <see cref="IRemoteCommand"/>. If no assemblies are provided, the calling assembly is used by default. </param>
        /// 
        /// <returns>
        /// The <see cref="IServiceCollection"/> instance with all discovered and registered <see cref="IRemoteCommand"/> implementations.
        /// </returns>
        ///
        /// <author>Almighty-Shogun</author>
        /// <since>1.2.0</since>
        public IServiceCollection RegisterRemoteCommands(params Assembly[] assemblies)
        {
            if (assemblies.Length == 0)
            {
                assemblies = [Assembly.GetCallingAssembly()];
            }
            
            return serviceCollection.RegisterOnInherit<IRemoteCommand>(ServiceLifetime.Transient, assemblies);
        }
    }
}
