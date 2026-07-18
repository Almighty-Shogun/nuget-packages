using System.Reflection;
using AlmightyShogun.Utils;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace AlmightyShogun.RemoteCommands;

/// <summary>
/// Registers remote command listener services and command implementations.
/// </summary>
///
/// <author>Almighty-Shogun</author>
/// <since>1.0.0</since>
public static class PackageRegistry
{
    /// <param name="serviceCollection">The service collection used to register dependencies related to remote command functionality.</param>
    extension(IServiceCollection serviceCollection)
    {
        /// <summary>
        /// Registers the remote command listener and binds its required configuration.
        /// </summary>
        ///
        /// <param name="configuration">The application configuration that contains the required <c>RemoteServer</c> section.</param>
        ///
        /// <returns>The <see cref="IServiceCollection"/> instance with the remote command listener registered.</returns>
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
        /// Registers remote command implementations in the specified assemblies with the DI container.
        /// </summary>
        ///
        /// <param name="assemblies">The assemblies to scan for types inheriting from <see cref="RemoteCommand{T}"/>. If none are provided, the calling assembly is used.</param>
        ///
        /// <returns>
        /// The <see cref="IServiceCollection"/> instance with all discovered and registered remote command implementations.
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
