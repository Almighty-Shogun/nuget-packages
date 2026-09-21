using System.Reflection;
using System.Runtime.CompilerServices;
using AlmightyShogun.Utils;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace AlmightyShogun.ConsoleCommands;

/// <summary>
/// Provides dependency injection registration for console commands.
/// </summary>
///
/// <author>Almighty-Shogun</author>
/// <since>2.1.0</since>
public static class ConsoleCommandExtensions
{
    /// <param name="serviceCollection">The service collection to register console commands with.</param>
    extension(IServiceCollection serviceCollection)
    {
        /// <summary>
        /// Registers the console command handler.
        /// </summary>
        ///
        /// <returns>The service collection.</returns>
        ///
        /// <author>Almighty-Shogun</author>
        /// <since>1.0.0</since>
        public IServiceCollection AddConsoleCommands() => serviceCollection.AddSingleton<IConsoleCommandHandler, ConsoleCommandHandler>();

        /// <summary>
        /// Registers console commands from the calling assembly.
        /// </summary>
        ///
        /// <returns>The service collection.</returns>
        ///
        /// <author>Almighty-Shogun</author>
        /// <since>4.0.0</since>
        [MethodImpl(MethodImplOptions.NoInlining)]
        public IServiceCollection RegisterConsoleCommands() => serviceCollection.RegisterConsoleCommands([Assembly.GetCallingAssembly()]);

        /// <summary>
        /// Discovers and registers console commands from the specified assemblies.
        /// </summary>
        ///
        /// <param name="assemblies">The assemblies to scan for console commands.</param>
        ///
        /// <returns>The service collection.</returns>
        ///
        /// <exception cref="InvalidOperationException">
        /// Thrown when a discovered command is invalid or when a command name or alias is claimed by multiple commands.
        /// </exception>
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
