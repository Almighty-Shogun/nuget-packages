using System.Reflection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace AlmightyShogun.Utils;

/// <summary>
/// Provides dependency-injection registration helpers for options, service registries, and assembly scanning.
/// </summary>
///
/// <author>Almighty-Shogun</author>
/// <since>1.0.0</since>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Provides dependency-injection extension methods for the target service collection.
    /// </summary>
    ///
    /// <param name="serviceCollection">The service collection that receives the registrations.</param>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>1.0.0</since>
    extension(IServiceCollection serviceCollection)
    {
        /// <summary>
        /// Adds and configures the service of type <typeparamref name="T"/> into the provided <see cref="IServiceCollection"/>.
        /// </summary>
        ///
        /// <typeparam name="T">The service type implementing <see cref="IServiceRegistry"/> to be added and configured.</typeparam>
        ///
        /// <returns>The updated <see cref="IServiceCollection"/> with the configured service.</returns>
        ///
        /// <author>Almighty-Shogun</author>
        /// <since>1.0.0</since>
        public IServiceCollection AddService<T>() where T : IServiceRegistry, new()
            => new T().ConfigureService(serviceCollection);

        /// <summary>
        /// Adds a strongly typed options class of type <typeparamref name="T"/> to the service collection
        /// and binds it to a configuration section.
        /// </summary>
        ///
        /// <typeparam name="T">The type of the options class to configure.</typeparam>
        /// <param name="section">The <see cref="IConfigurationSection"/> from which the options will be bound.</param>
        ///
        /// <returns>The <see cref="IServiceCollection"/> instance with the options binding configured.</returns>
        ///
        /// <author>Almighty-Shogun</author>
        /// <since>1.0.0</since>
        public IServiceCollection AddConfiguration<T>(IConfigurationSection section) where T : class
        {
            serviceCollection.AddOptions<T>()
                .Bind(section)
                .ValidateDataAnnotations()
                .ValidateOnStart();

            return serviceCollection;
        }

        /// <summary>
        /// Registers concrete types that inherit from or implement <typeparamref name="T"/> using <typeparamref name="T"/> as the service type.
        /// </summary>
        ///
        /// <typeparam name="T">The base type or interface to search for inheriting or implementing types.</typeparam>
        /// <param name="serviceLifetime">The lifetime of the registered services. Defaults to <see cref="ServiceLifetime.Singleton"/>.</param>
        /// <param name="assemblies">The assemblies to search. If none are provided, the calling assembly is used.</param>
        ///
        /// <returns>The <see cref="IServiceCollection"/> instance with matching implementations registered.</returns>
        ///
        /// <author>Almighty-Shogun</author>
        /// <since>1.0.0</since>
        public IServiceCollection RegisterOnInherit<T>(ServiceLifetime serviceLifetime = ServiceLifetime.Singleton, params Assembly[] assemblies) where T : class
        {
            if (assemblies.Length == 0)
            {
                assemblies = [Assembly.GetCallingAssembly()];
            }

            return serviceCollection.InternalRegister<T>(serviceLifetime, true, assemblies);
        }

        /// <summary>
        /// Registers concrete types that inherit from or implement <typeparamref name="T"/> with configurable service type behavior.
        /// </summary>
        ///
        /// <typeparam name="T">The base type to search for inheriting types.</typeparam>
        /// <param name="addType">Whether to register each implementation under <typeparamref name="T"/> instead of its concrete type.</param>
        /// <param name="serviceLifetime">The lifetime to use for each registration.</param>
        /// <param name="assemblies">The assemblies to search. If none are provided, the calling assembly is used.</param>
        ///
        /// <returns>The <see cref="IServiceCollection"/> instance with matching implementations registered.</returns>
        ///
        /// <author>Almighty-Shogun</author>
        /// <since>1.0.0</since>
        public IServiceCollection RegisterOnInherit<T>(bool addType, ServiceLifetime serviceLifetime = ServiceLifetime.Singleton, params Assembly[] assemblies) where T : class
        {
            if (assemblies.Length == 0)
            {
                assemblies = [Assembly.GetCallingAssembly()];
            }

            return serviceCollection.InternalRegister<T>(serviceLifetime, addType, assemblies);
        }

        /// <summary>
        /// Registers discovered implementation types using either <typeparamref name="T"/> or the concrete type as the service type.
        /// </summary>
        ///
        /// <typeparam name="T">The base type or interface to search for inheriting or implementing types.</typeparam>
        /// <param name="serviceLifetime">The lifetime to use for each registration.</param>
        /// <param name="addType">Whether to register each implementation under <typeparamref name="T"/> instead of its concrete type.</param>
        /// <param name="assemblies">The assemblies to scan for implementations.</param>
        ///
        /// <returns>The <see cref="IServiceCollection"/> instance with matching implementations registered.</returns>
        ///
        /// <author>Almighty-Shogun</author>
        /// <since>1.0.0</since>
        private IServiceCollection InternalRegister<T>(ServiceLifetime serviceLifetime, bool addType, params Assembly[] assemblies) where T : class
        {
            IEnumerable<Type> types = ApplicationUtils.GetOnInherit<T>(assemblies)
                .Where(t => t is { IsInterface: false, IsAbstract: false });

            foreach (Type type in types)
            {
                Type serviceType = addType ? typeof(T) : type;
                serviceCollection.Add(new ServiceDescriptor(serviceType, type, serviceLifetime));
            }

            return serviceCollection;
        }
    }
}
