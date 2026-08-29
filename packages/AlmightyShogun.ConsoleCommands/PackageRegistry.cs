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
    /// <param name="serviceCollection">
    /// The service collection the console command services and command classes are registered into.
    /// </param>
    extension(IServiceCollection serviceCollection)
    {
        /// <summary>
        /// Registers the console command handler service. It only runs the input loop, so the command classes it dispatches to
        /// still have to be registered with <see cref="RegisterConsoleCommands(IServiceCollection)"/>.
        /// </summary>
        ///
        /// <returns>The <see cref="IServiceCollection"/> instance with the console command handler registered.</returns>
        ///
        /// <author>Almighty-Shogun</author>
        /// <since>1.0.0</since>
        public IServiceCollection AddConsoleCommands() => serviceCollection.AddSingleton<IConsoleCommandHandler, ConsoleCommandHandler>();

        /// <summary>
        /// Registers the command classes declared in the calling assembly, which is the usual case when the commands live in
        /// the startup project. Reach for the overload taking assemblies when they do not.
        /// </summary>
        ///
        /// <returns>The <see cref="IServiceCollection"/> instance with the console commands registered.</returns>
        ///
        /// <author>Almighty-Shogun</author>
        /// <since>Unreleased</since>
        public IServiceCollection RegisterConsoleCommands() => serviceCollection.RegisterConsoleCommands([Assembly.GetCallingAssembly()]);

        /// <summary>
        /// Registers the command classes declared in the given assemblies as transient services under their own concrete
        /// type, alongside a descriptor naming each one, so a fresh instance is built for each invocation from a fresh scope
        /// and a command may depend on scoped application services.
        /// </summary>
        ///
        /// <param name="assemblies">
        /// The assemblies to scan, in the order they should be searched. An empty array registers nothing; the overload taking
        /// no assembly at all is the one that falls back to the calling assembly.
        /// </param>
        ///
        /// <returns>The <see cref="IServiceCollection"/> instance with the console commands registered.</returns>
        ///
        /// <exception cref="InvalidOperationException">
        /// A discovered class breaks one of the command rules: no <see cref="ConsoleCommandAttribute"/>, a name that cannot
        /// be typed, or anything other than one public <c>ExecuteAsync</c> returning an awaitable. Raised here so the
        /// offending class is named at startup rather than quietly never answering the prompt.
        /// </exception>
        ///
        /// <remarks>
        /// Commands are registered under their concrete type rather than under the command interface, because the dispatcher
        /// resolves one by type from a per-invocation scope instead of enumerating them all. A class carrying
        /// <see cref="SkipAutoRegistrationAttribute"/> is skipped, and a name claimed twice is reported when the dispatcher
        /// builds its table rather than here.
        /// </remarks>
        ///
        /// <author>Almighty-Shogun</author>
        /// <since>1.1.0</since>
        public IServiceCollection RegisterConsoleCommands(Assembly[] assemblies)
        {
            serviceCollection.RegisterOnInherit<IConsoleCommand>(assemblies, ServiceLifetime.Transient, false);

            IEnumerable<Type> commandTypes = ConsoleCommandDiscovery.GetConsoleCommandTypes(assemblies)
                .Where(type => !type.IsDefined(typeof(SkipAutoRegistrationAttribute), false));

            foreach (Type commandType in commandTypes)
            {
                (ConsoleCommandAttribute attribute, _) = CommandMetadata.Describe(commandType);

                serviceCollection.AddSingleton(new ConsoleCommandDescriptor(
                    attribute.Name,
                    commandType.GetCustomAttribute<AliasAttribute>()?.Aliases ?? [],
                    commandType
                ));
            }

            return serviceCollection;
        }
    }
}
