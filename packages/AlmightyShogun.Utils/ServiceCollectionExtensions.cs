using System.Reflection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace AlmightyShogun.Utils;

public static class ServiceCollectionExtensions
{
    extension(IServiceCollection serviceCollection)
    {
        /// <summary>
        /// Adds and configures the service of type <typeparamref name="T"/> into the provided <see cref="IServiceCollection"/>.
        /// </summary>
        /// 
        /// <typeparam name="T">The service type implementing <see cref="IService"/> to be added and configured.</typeparam>
        /// 
        /// <returns>The updated <see cref="IServiceCollection"/> with the configured service.</returns>
        /// 
        /// <author>Almighty-Shogun</author>
        /// <since>1.0.0</since>
        public IServiceCollection AddService<T>() where T : IService, new()
        {
            return new T().ConfigureService(serviceCollection);
        }
        
        /// <summary>
        /// Adds a strongly typed options class of type <typeparamref name="T"/> to the service collection
        /// and binds it to a configuration section.
        /// </summary>
        /// 
        /// <typeparam name="T">The type of the options class to configure.</typeparam>
        /// <param name="section">The <see cref="IConfigurationSection"/> from which the options will be bound.</param>
        ///
        /// <returns>The updated <see cref="IServiceCollection"/> with the configured configuration.</returns>
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
        /// Registers services of types inheriting or implementing the specified type <typeparamref name="T"/> into the provided <see cref="IServiceCollection"/>.
        /// </summary>
        /// 
        /// <typeparam name="T">The base type or interface to search for inheriting or implementing types.</typeparam>
        /// <param name="serviceLifetime">The lifetime of the registered services. Defaults to <see cref="ServiceLifetime.Singleton"/>.</param>
        /// <param name="assemblies">The assemblies to search for inheriting or implementing types. If not specified, the calling assembly is used.</param>
        /// 
        /// <returns>The updated <see cref="IServiceCollection"/> with the registered services.</returns>
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
        /// Registers all types inheriting from the specified base type <typeparamref name="T"/> found in the given assemblies with the provided <see cref="IServiceCollection"/>.
        /// </summary>
        /// 
        /// <typeparam name="T">The base type to search for inheriting types.</typeparam>
        /// <param name="addType">Specifies whether the base type <typeparamref name="T"/> itself should also be registered.</param>
        /// <param name="serviceLifetime">The service lifetime to be used for the registrations (e.g., Singleton, Scoped, Transient).</param>
        /// <param name="assemblies">The assemblies to search for types inheriting from <typeparamref name="T"/>. If not specified, the calling assembly is used by default.</param>
        /// 
        /// <returns>The updated <see cref="IServiceCollection"/> containing the registered services.</returns>
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
        /// Registers types that inherit from or implement the specified base type or interface <typeparamref name="T"/>
        /// into the service collection with the specified service lifetime and optional service type addition.
        /// </summary>
        /// 
        /// <typeparam name="T">The base type or interface to search for inheriting or implementing types.</typeparam>
        /// <param name="serviceLifetime">The lifetime of the registered services (e.g., Singleton, Scoped, Transient).</param>
        /// <param name="addType">Indicates whether to register the base type or interface <typeparamref name="T"/> as the service type.</param>
        /// <param name="assemblies">The assemblies to scan for types inheriting from or implementing <typeparamref name="T"/>.</param>
        /// 
        /// <returns>The updated <see cref="IServiceCollection"/> with the registered services.</returns>
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
