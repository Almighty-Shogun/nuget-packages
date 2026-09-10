using System.Reflection;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace AlmightyShogun.Utils;

/// <summary>
/// Provides extension methods for configuring services in an <see cref="IServiceCollection"/>.
/// </summary>
///
/// <author>Almighty-Shogun</author>
/// <since>1.0.0</since>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Provides extension methods for configuring the service collection.
    /// </summary>
    ///
    /// <param name="serviceCollection">
    /// The service collection to configure.
    /// </param>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>1.0.0</since>
    extension(IServiceCollection serviceCollection)
    {
        /// <summary>
        /// Registers the services using the specified service registry.
        /// </summary>
        ///
        /// <typeparam name="T">
        /// The service registry to use.
        /// </typeparam>
        ///
        /// <returns>The configured service collection.</returns>
        ///
        /// <author>Almighty-Shogun</author>
        /// <since>1.0.0</since>
        public IServiceCollection AddService<T>() where T : IServiceRegistry, new()
        {
            new T().ConfigureService(serviceCollection);

            return serviceCollection;
        }

        /// <summary>
        /// Binds a configuration section to options of type <typeparamref name="T"/>.
        /// </summary>
        ///
        /// <typeparam name="T">
        /// The options type to configure.
        /// </typeparam>
        /// <param name="section">
        /// The configuration section to bind.
        /// </param>
        /// 
        /// <param name="validateDataAnnotations">
        /// Whether to validate the options using data annotations.
        /// </param>
        /// <param name="validateOnStart">
        /// Whether to validate options when the application starts.
        /// </param>
        ///
        /// <returns>The configured service collection.</returns>
        ///
        /// <author>Almighty-Shogun</author>
        /// <since>1.0.0</since>
        public IServiceCollection AddConfiguration<T>(
            IConfigurationSection section,
            bool validateDataAnnotations = true,
            bool validateOnStart = true
        ) where T : class
        {
            OptionsBuilder<T> optionsBuilder = serviceCollection.AddOptions<T>().Bind(section);

            if (validateDataAnnotations)
                optionsBuilder.ValidateDataAnnotations();

            if (validateOnStart)
                optionsBuilder.ValidateOnStart();

            return serviceCollection;
        }

        /// <summary>
        /// Replaces the first registration of <typeparamref name="TService"/> with
        /// <typeparamref name="TImplementation"/>.
        /// </summary>
        ///
        /// <typeparam name="TService">
        /// The service type to replace.
        /// </typeparam>
        /// 
        /// <typeparam name="TImplementation">
        /// The replacement implementation type.
        /// </typeparam>
        /// 
        /// <param name="serviceLifetime">
        /// The lifetime of the replacement service.
        /// </param>
        ///
        /// <returns>The configured service collection.</returns>
        ///
        /// <author>Almighty-Shogun</author>
        /// <since>4.0.0</since>
        public IServiceCollection ReplaceService<TService, TImplementation>(
            ServiceLifetime serviceLifetime = ServiceLifetime.Singleton
        ) where TService : class where TImplementation : class, TService
            => serviceCollection.Replace(ServiceDescriptor.Describe(typeof(TService), typeof(TImplementation), serviceLifetime));

        /// <summary>
        /// Registers concrete types in the calling assembly that are assignable to <typeparamref name="T"/>.
        /// </summary>
        ///
        /// <typeparam name="T">
        /// The service type to register.
        /// </typeparam>
        /// <param name="serviceLifetime">The lifetime of the registered services.</param>
        ///
        /// <returns>The configured service collection.</returns>
        ///
        /// <author>Almighty-Shogun</author>
        /// <since>4.0.0</since>
        public IServiceCollection RegisterOnInherit<T>(ServiceLifetime serviceLifetime = ServiceLifetime.Singleton) where T : class
            => serviceCollection.RegisterOnInherit<T>([Assembly.GetCallingAssembly()], serviceLifetime);

        /// <summary>
        /// Registers concrete types in the specified assemblies that are assignable to <typeparamref name="T"/>. 
        /// </summary>
        ///
        /// <typeparam name="T">
        /// The type to which discovered implementations must be assignable.
        /// </typeparam>
        /// <param name="assemblies">
        /// The assemblies to scan.
        /// </param>
        /// <param name="serviceLifetime">The lifetime of the registered services.</param>
        /// <param name="registerAsBaseType">
        /// Whether to register each implementation as <typeparamref name="T"/> rather than as its concrete type.
        /// </param>
        /// <param name="filter">
        /// An optional predicate used to filter discovered types.
        /// </param>
        ///
        /// <returns>The configured service collection.</returns>
        ///
        /// <author>Almighty-Shogun</author>
        /// <since>1.0.0</since>
        public IServiceCollection RegisterOnInherit<T>(
            Assembly[] assemblies,
            ServiceLifetime serviceLifetime = ServiceLifetime.Singleton,
            bool registerAsBaseType = true,
            Func<Type, bool>? filter = null
        ) where T : class => serviceCollection.InternalRegister<T>(serviceLifetime, registerAsBaseType, filter, assemblies);

        /// <summary>
        /// Discovers and registers implementations of <typeparamref name="T"/> from the specified assemblies.
        /// </summary>
        ///
        /// <typeparam name="T">The type to which discovered implementations must be assignable.</typeparam>
        /// <param name="serviceLifetime">The lifetime of the registered services.</param>
        /// <param name="registerAsBaseType">
        /// Whether to register each implementation under <typeparamref name="T"/> rather than as its concrete type.
        /// </param>
        /// <param name="filter">An optional predicate used to filter discovered types.</param>
        /// <param name="assemblies">
        /// The assemblies to scan.
        /// </param>
        ///
        /// <returns>The configured service collection.</returns>
        ///
        /// <author>Almighty-Shogun</author>
        /// <since>1.0.0</since>
        private IServiceCollection InternalRegister<T>(
            ServiceLifetime serviceLifetime,
            bool registerAsBaseType,
            Func<Type, bool>? filter,
            Assembly[] assemblies
        ) where T : class
        {
            IEnumerable<Type> types = TypeDiscovery.FindAssignableTypes<T>(assemblies)
                .Where(t => !t.IsDefined(typeof(SkipAutoRegistrationAttribute)))
                .Where(t => filter is null || filter(t));

            foreach (Type type in types)
            {
                Type serviceType = registerAsBaseType ? typeof(T) : type;
                var descriptor = new ServiceDescriptor(serviceType, type, serviceLifetime);

                if (registerAsBaseType)
                    serviceCollection.TryAddEnumerable(descriptor);
                else
                    serviceCollection.TryAdd(descriptor);
            }

            return serviceCollection;
        }
    }
}
