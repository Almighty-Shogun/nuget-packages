using System.Reflection;
using AlmightyShogun.Utils;
using Microsoft.Extensions.DependencyInjection;

namespace AlmightyShogun.ConsoleCommands;

public static class PackageRegistry
{
    /// <param name="serviceCollection">The service collection used to register dependencies related to console command functionality.</param>
    extension(IServiceCollection serviceCollection)
    {
        /// <summary>
        /// Registers console command services with the specified <see cref="IServiceCollection"/>.
        /// </summary>
        /// <returns>The <see cref="IServiceCollection"/> instance with the console commands handlers registered.</returns>
        /// 
        /// <author>Almighty-Shogun</author>
        /// <since>1.0.0</since>
        public IServiceCollection AddConsoleCommands()
        {
            return serviceCollection.AddSingleton<IConsoleCommandHandler, ConsoleCommandHandler>();
        }

        /// <summary>
        /// Registers console command services for the specified assemblies using the provided <see cref="IServiceCollection"/>.
        /// </summary>
        /// 
        /// <param name="assemblies">An array of <see cref="Assembly"/> objects from which console commands should be loaded. If no assemblies are provided, the calling assembly is used by default.</param>
        ///
        /// <returns>The <see cref="IServiceCollection"/> instance with the console commands registered.</returns>
        ///
        /// <author>Almighty-Shogun</author>
        /// <since>1.0.0</since>
        public IServiceCollection RegisterConsoleCommands(params Assembly[] assemblies)
        {
            if (assemblies.Length == 0)
            {
                assemblies = [Assembly.GetCallingAssembly()];
            }
            
            return serviceCollection.RegisterOnInherit<IConsoleCommand>(ServiceLifetime.Transient, assemblies);
        }
    }
}
