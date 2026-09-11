using System.Reflection;
using System.Runtime.CompilerServices;
using AlmightyShogun.Utils;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace AlmightyShogun.RemoteCommands;

/// <summary>
/// Provides remote command registration extensions.
/// </summary>
///
/// <author>Almighty-Shogun</author>
/// <since>1.0.0</since>
public static class RemoteCommandExtensions
{
    /// <param name="serviceCollection">The service collection to configure.</param>
    extension(IServiceCollection serviceCollection)
    {
        /// <summary>
        /// Registers the remote command server.
        /// </summary>
        ///
        /// <param name="configuration">
        /// The application configuration.
        /// </param>
        ///
        /// <returns>The configured service collection.</returns>
        ///
        /// <author>Almighty-Shogun</author>
        /// <since>1.0.0</since>
        public IServiceCollection AddRemoteCommands(IConfiguration configuration) => serviceCollection
            .AddConfiguration<RemoteServerSettings>(configuration.GetSection("RemoteServer"))
            .AddSingleton<IRemoteCommandHandler, RemoteCommandHandler>();

        /// <summary>
        /// Registers remote commands from the calling assembly.
        /// </summary>
        ///
        /// <returns>The configured service collection.</returns>
        ///
        /// <exception cref="InvalidOperationException">
        /// A discovered command is not marked with <see cref="RemoteCommandAttribute"/>.
        /// </exception>
        ///
        /// <author>Almighty-Shogun</author>
        /// <since>4.0.0</since>
        [MethodImpl(MethodImplOptions.NoInlining)]
        public IServiceCollection RegisterRemoteCommands() => serviceCollection.RegisterRemoteCommands([Assembly.GetCallingAssembly()]);

        /// <summary>
        /// Registers remote commands from the specified assemblies.
        /// </summary>
        ///
        /// <param name="assemblies">
        /// The assemblies to scan.
        /// </param>
        ///
        /// <returns>The configured service collection.</returns>
        ///
        /// <exception cref="InvalidOperationException">
        /// A discovered command is not marked with <see cref="RemoteCommandAttribute"/>.
        /// </exception>
        ///
        /// <author>Almighty-Shogun</author>
        /// <since>1.2.0</since>
        public IServiceCollection RegisterRemoteCommands(Assembly[] assemblies)
        {
            serviceCollection.RegisterOnInherit<IRemoteCommand>(assemblies, ServiceLifetime.Transient, false);

            IEnumerable<Type> commandTypes = TypeDiscovery
                .FindAssignableTypes<IRemoteCommand>(assemblies)
                .Where(type => !type.IsDefined(typeof(SkipAutoRegistrationAttribute), false));

            foreach (Type commandType in commandTypes)
            {
                var attribute = commandType.GetCustomAttribute<RemoteCommandAttribute>();

                if (attribute is null)
                    throw new InvalidOperationException($"Command {commandType.Name} must have {nameof(RemoteCommandAttribute)}.");
                
                serviceCollection.AddSingleton(new RemoteCommandDescriptor(attribute.Name, commandType));
            }

            return serviceCollection;
        }
    }
}
