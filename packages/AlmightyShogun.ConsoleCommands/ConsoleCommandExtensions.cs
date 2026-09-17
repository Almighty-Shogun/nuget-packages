using System.Reflection;
using System.Runtime.CompilerServices;
using AlmightyShogun.Utils;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace AlmightyShogun.ConsoleCommands;

/// <summary>
/// Registers console command services and command implementations in dependency injection.
/// </summary>
///
/// <author>Almighty-Shogun</author>
/// <since>2.1.0</since>
public static class ConsoleCommandExtensions
{
    /// <param name="serviceCollection">
    /// The service collection the console command services and command classes are registered into.
    /// </param>
    extension(IServiceCollection serviceCollection)
    {
        /// <summary>
        /// Registers the console command handler as a singleton. It only runs the input loop, so the command classes it
        /// dispatches to still have to be registered with <see cref="RegisterConsoleCommands(IServiceCollection)"/>.
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
        /// <since>4.0.0</since>
        [MethodImpl(MethodImplOptions.NoInlining)]
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
        /// A discovered class breaks one of the rules named on <see cref="ConsoleCommandBase"/>. Raised here so the offending
        /// class is named at startup rather than quietly never answering the prompt.
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
            IEnumerable<Type> commandTypes = ConsoleCommandDiscovery.GetConsoleCommandTypes(assemblies)
                .Where(type => !type.IsDefined(typeof(SkipAutoRegistrationAttribute), false));

            ConsoleCommandDescriptor[] existingDescriptors =
            [
                .. serviceCollection
                    .Where(descriptor => descriptor.ServiceType == typeof(ConsoleCommandDescriptor))
                    .Select(descriptor => descriptor.ImplementationInstance)
                    .OfType<ConsoleCommandDescriptor>()
            ];

            HashSet<Type> registeredTypes =
            [
                .. existingDescriptors
                    .Select(descriptor => descriptor.ImplementationType)
            ];

            var claimedNames = new Dictionary<string, Type>(StringComparer.OrdinalIgnoreCase);

            foreach (ConsoleCommandDescriptor descriptor in existingDescriptors)
            {
                Claim(descriptor.Name, descriptor.ImplementationType, claimedNames);

                foreach (string alias in descriptor.Aliases)
                {
                    Claim(alias, descriptor.ImplementationType, claimedNames);
                }
            }

            var commands = new List<ConsoleCommandDescriptor>();

            foreach (Type commandType in commandTypes)
            {
                if (registeredTypes.Contains(commandType))
                    continue;

                (ConsoleCommandAttribute attribute, _) = CommandMetadata.Describe(commandType);

                IReadOnlyList<string> aliases = commandType.GetCustomAttribute<AliasAttribute>()?.Aliases ?? [];

                Claim(attribute.Name, commandType, claimedNames);

                foreach (string alias in aliases)
                {
                    Claim(alias, commandType, claimedNames);
                }

                commands.Add(new ConsoleCommandDescriptor(attribute.Name, aliases, commandType));
            }

            foreach (ConsoleCommandDescriptor command in commands)
            {
                serviceCollection.TryAdd(
                    new ServiceDescriptor(command.ImplementationType, command.ImplementationType, ServiceLifetime.Transient));

                serviceCollection.AddSingleton(command);
            }

            return serviceCollection;
        }
    }

    private static void Claim(string name, Type implementationType, Dictionary<string, Type> claimedNames)
    {
        if (!claimedNames.TryAdd(name, implementationType) && claimedNames[name] != implementationType)
            throw new InvalidOperationException(
                $"Command name or alias '{name}' is claimed by both " +
                $"{claimedNames[name].Name} and {implementationType.Name}.");
    }
}
