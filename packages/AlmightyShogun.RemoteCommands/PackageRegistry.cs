using System.Reflection;
using AlmightyShogun.Core;
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
        /// Registers the command classes declared in the given assemblies as transient services, so a fresh instance is
        /// built per request and a command may depend on scoped application services.
        /// </summary>
        ///
        /// <param name="assemblies">
        /// The assemblies to scan, in the order they should be searched. An empty array registers nothing; the overload
        /// taking no assembly at all is the one that falls back to the calling assembly.
        /// </param>
        ///
        /// <returns>The <see cref="IServiceCollection"/> instance with the remote commands registered.</returns>
        ///
        /// <remarks>
        /// Every discovered command class is registered, including one that is malformed. The resulting
        /// <see cref="InvalidOperationException"/> from <see cref="RemoteCommand{T}"/> then surfaces when the listener is
        /// resolved, which names the offending class rather than silently leaving its command name unreachable.
        /// </remarks>
        ///
        /// <author>Almighty-Shogun</author>
        /// <since>1.2.0</since>
        public IServiceCollection RegisterRemoteCommands(Assembly[] assemblies)
            => serviceCollection.RegisterOnInherit<IRemoteCommand>(assemblies, ServiceLifetime.Transient);
    }
}
