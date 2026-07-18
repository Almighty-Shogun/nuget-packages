using System.Reflection;
using AlmightyShogun.Utils;
using Microsoft.Extensions.DependencyInjection;

namespace AlmightyShogun.ConsoleCommands;

/// <summary>
/// Registers console command services and command implementations in dependency injection.
/// </summary>
///
/// <author>Almighty-Shogun</author>
/// <since>2.1.0</since>
public static class PackageRegistry
{
    /// <param name="serviceCollection">The service collection used to register dependencies related to console command functionality.</param>
    extension(IServiceCollection serviceCollection)
    {
        /// <summary>
        /// Registers the console command handler service.
        /// </summary>
        ///
        /// <returns>The <see cref="IServiceCollection"/> instance with the console command handler registered.</returns>
        ///
        /// <author>Almighty-Shogun</author>
        /// <since>1.0.0</since>
        public IServiceCollection AddConsoleCommands()
        {
            return serviceCollection.AddSingleton<IConsoleCommandHandler, ConsoleCommandHandler>();
        }

        /// <summary>
        /// Registers command classes discovered in the provided assemblies.
        /// </summary>
        ///
        /// <param name="assemblies">The assemblies to scan for command classes. If none are provided, the calling assembly is used.</param>
        ///
        /// <returns>The <see cref="IServiceCollection"/> instance with the console commands registered.</returns>
        ///
        /// <author>Almighty-Shogun</author>
        /// <since>1.1.0</since>
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
