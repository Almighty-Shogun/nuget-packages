using System.Reflection;
using AlmightyShogun.Utils;
using Microsoft.Extensions.DependencyInjection;
using AlmightyShogun.RemoteCommands.Configuration;

namespace AlmightyShogun.RemoteCommands;

public static class ServiceCollectionExtensions
{
    extension(IServiceCollection serviceCollection)
    {
        /// <summary>
        /// Registers remote command services with the specified <see cref="IServiceCollection"/>.
        /// </summary>
        /// 
        /// <param name="config">An optional action used to configure the remote commands connection handler.</param>
        /// 
        /// <returns>The <see cref="IServiceCollection"/> instance with the remote commands handlers registered.</returns>
        /// 
        /// <author>Almighty-Shogun</author>
        /// <since>1.0.0</since>
        public IServiceCollection AddRemoteCommands(Action<IRemoteConfig>? config = null)
        {
            if (config is not null)
            {
                RemoteConfig remoteConfig = new();
            
                config(remoteConfig);

                serviceCollection.AddSingleton<IRemoteConfig>(remoteConfig);
            }
            else
            {
                serviceCollection.AddSingleton<IRemoteConfig, RemoteConfig>();
            }
        
            return serviceCollection.AddSingleton<IRemoteCommandHandler, RemoteCommandHandler>();
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
