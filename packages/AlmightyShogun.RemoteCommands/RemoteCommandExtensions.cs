using System.Reflection;
using AlmightyShogun.Utils;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace AlmightyShogun.RemoteCommands;

/// <summary>
/// Registers the remote command listener and the command implementations it dispatches to.
/// </summary>
///
/// <author>Almighty-Shogun</author>
/// <since>1.0.0</since>
public static class PackageRegistry
{
    /// <param name="serviceCollection">
    /// The service collection the listener and the discovered command classes are registered into.
    /// </param>
    extension(IServiceCollection serviceCollection)
    {
        /// <summary>
        /// Registers the listener and binds the <c>RemoteServer</c> section it needs. A port or timeout outside its range
        /// stops the host from starting; an unparseable address or whitelist entry stops the listener being resolved.
        /// </summary>
        ///
        /// <param name="configuration">
        /// The application configuration. Read for a <c>RemoteServer</c> section, which is required: <c>Port</c> has no
        /// default, so an absent section fails validation rather than binding a listener nobody asked for.
        /// </param>
        ///
        /// <returns>The <see cref="IServiceCollection"/> instance with the remote command listener registered.</returns>
        ///
        /// <author>Almighty-Shogun</author>
        /// <since>1.0.0</since>
        public IServiceCollection AddRemoteCommands(IConfiguration configuration) => serviceCollection
            .AddConfiguration<RemoteServerSettings>(configuration.GetSection("RemoteServer"))
            .AddSingleton<IRemoteCommandHandler, RemoteCommandHandler>();

        /// <summary>
        /// Registers the command classes declared in the calling assembly, which is the usual case when the commands
        /// live in the startup project. Reach for the overload taking assemblies when they do not.
        /// </summary>
        ///
        /// <returns>The <see cref="IServiceCollection"/> instance with the remote commands registered.</returns>
        ///
        /// <author>Almighty-Shogun</author>
        /// <since>Unreleased</since>
        public IServiceCollection RegisterRemoteCommands() => serviceCollection.RegisterRemoteCommands([Assembly.GetCallingAssembly()]);

        /// <summary>
        /// Registers the command classes declared in the given assemblies as transient services under their own concrete
        /// type, alongside a descriptor naming each one, so a fresh instance is built per request from a fresh scope and a
        /// command may depend on scoped application services.
        /// </summary>
        ///
        /// <param name="assemblies">
        /// The assemblies to scan, in the order they should be searched. An empty array registers nothing; the overload
        /// taking no assembly at all is the one that falls back to the calling assembly.
        /// </param>
        ///
        /// <returns>The <see cref="IServiceCollection"/> instance with the remote commands registered.</returns>
        ///
        /// <exception cref="InvalidOperationException">
        /// A discovered command class carries no <see cref="RemoteCommandAttribute"/> and so declares no name to be
        /// reachable by. Raised here rather than when the listener is resolved, because the name is what a descriptor is
        /// built from.
        /// </exception>
        ///
        /// <remarks>
        /// Commands are registered under their concrete type rather than under the command interface, because the listener
        /// resolves one by type from a per-request scope instead of enumerating them all. A class carrying
        /// <see cref="SkipAutoRegistrationAttribute"/> is skipped, and a name claimed twice is reported when the dispatch
        /// table is built rather than here.
        /// </remarks>
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
