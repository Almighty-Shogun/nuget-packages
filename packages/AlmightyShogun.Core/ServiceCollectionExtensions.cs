using System.Reflection;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace AlmightyShogun.Core;

/// <summary>
/// Provides the registration helpers this package contributes to startup: binding a validated options class, running a
/// reusable registration module, and discovering implementations across assemblies instead of listing them by hand.
/// </summary>
///
/// <author>Almighty-Shogun</author>
/// <since>1.0.0</since>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Provides the registration helpers as extensions on the collection being built during startup.
    /// </summary>
    ///
    /// <param name="serviceCollection">
    /// The collection that receives the registrations. Every helper returns it so calls can be chained.
    /// </param>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>1.0.0</since>
    extension(IServiceCollection serviceCollection)
    {
        /// <summary>
        /// Runs a registration module, letting a feature keep its wiring in one reusable type instead of spreading it across
        /// startup. The module is constructed directly rather than resolved, so it cannot take constructor dependencies.
        /// </summary>
        ///
        /// <typeparam name="T">
        /// The module to run. It must implement <see cref="IServiceRegistry"/> and expose a public parameterless constructor,
        /// which the <c>new()</c> constraint enforces at compile time rather than at startup.
        /// </typeparam>
        ///
        /// <returns>
        /// The collection returned by <see cref="IServiceRegistry.ConfigureService"/>, which is normally the same instance
        /// that was passed in.
        /// </returns>
        ///
        /// <author>Almighty-Shogun</author>
        /// <since>1.0.0</since>
        public IServiceCollection AddService<T>() where T : IServiceRegistry, new() => new T().ConfigureService(serviceCollection);

        /// <summary>
        /// Binds a configuration section to a strongly typed options class and validates it, so a missing or malformed setting
        /// stops the application at startup with a message naming the offending property rather than failing later.
        /// </summary>
        ///
        /// <typeparam name="T">The options class to bind. Resolved afterward through <see cref="IOptions{TOptions}"/>.</typeparam>
        /// <param name="section">
        /// The configuration section to bind from. An absent section binds successfully and leaves every property at its
        /// default, which is why validation rather than binding is what catches a missing configuration.
        /// </param>
        /// <param name="validateDataAnnotations">
        /// Whether to enforce the data annotations declared on <typeparamref name="T"/>. Pass <c>false</c> for a settings type
        /// that is legitimately partial.
        /// </param>
        /// <param name="validateOnStart">
        /// Whether to validate while the host starts rather than the first time the options are resolved. Pass <c>false</c> to
        /// defer the failure to first resolution.
        /// </param>
        ///
        /// <returns>The <see cref="IServiceCollection"/> instance with the options binding configured.</returns>
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
        /// Swaps whatever is registered for <typeparamref name="TService"/> with
        /// <typeparamref name="TImplementation"/>, for overriding a default that a framework or another package has
        /// already registered.
        /// </summary>
        ///
        /// <typeparam name="TService">
        /// The service type whose existing registration is discarded. Only the first registration is replaced, so this is
        /// the wrong tool for a service registered many times and resolved as a sequence.
        /// </typeparam>
        /// <typeparam name="TImplementation">The implementation registered in its place.</typeparam>
        /// <param name="serviceLifetime">The lifetime applied to the replacement, which need not match the one it displaces.</param>
        ///
        /// <returns>The <see cref="IServiceCollection"/> instance with the registration replaced.</returns>
        ///
        /// <remarks>
        /// Adds the registration when nothing was there to replace, so it is safe to call before the default is
        /// registered. Order still matters the other way around: a later plain <c>Add</c> for the same service wins when
        /// the service is resolved singly, because the last registration is the one returned.
        /// </remarks>
        ///
        /// <author>Almighty-Shogun</author>
        /// <since>Unreleased</since>
        public IServiceCollection ReplaceService<TService, TImplementation>(
            ServiceLifetime serviceLifetime = ServiceLifetime.Singleton
        ) where TService : class where TImplementation : class, TService
            => serviceCollection.Replace(ServiceDescriptor.Describe(typeof(TService), typeof(TImplementation), serviceLifetime));

        /// <summary>
        /// Registers every concrete type assignable to <typeparamref name="T"/> in the calling assembly, under
        /// <typeparamref name="T"/> and with no filter. The shortest form, for the common case where the implementations sit
        /// beside the startup code that registers them.
        /// </summary>
        ///
        /// <typeparam name="T">
        /// The base type or interface to match, and the service type each implementation is registered under.
        /// </typeparam>
        /// <param name="serviceLifetime">The lifetime applied to every registration this call produces.</param>
        ///
        /// <returns>The <see cref="IServiceCollection"/> instance with matching implementations registered.</returns>
        ///
        /// <remarks>
        /// The assembly is resolved from the call stack, so it is whichever assembly contains the code that called this, not
        /// the one that started the process. Name the assembly explicitly when registering from a shared startup helper.
        /// </remarks>
        ///
        /// <author>Almighty-Shogun</author>
        /// <since>Unreleased</since>
        public IServiceCollection RegisterOnInherit<T>(ServiceLifetime serviceLifetime = ServiceLifetime.Singleton) where T : class
            => serviceCollection.RegisterOnInherit<T>([Assembly.GetCallingAssembly()], serviceLifetime);

        /// <summary>
        /// Registers every concrete type assignable to <typeparamref name="T"/> found in the given assemblies, for command
        /// handlers, jobs, rules, and anything else better discovered than listed by hand, across as many assemblies as the
        /// implementations are spread over.
        /// </summary>
        ///
        /// <typeparam name="T">
        /// The base type or interface to match, and by default the service type each implementation is registered under.
        /// </typeparam>
        /// <param name="assemblies">
        /// The assemblies to scan, in the order they should be searched. An empty array registers nothing; the overload that
        /// takes no assembly at all is the one that falls back to the calling assembly.
        /// </param>
        /// <param name="serviceLifetime">The lifetime applied to every registration this call produces.</param>
        /// <param name="addType">
        /// Whether to register each implementation under <typeparamref name="T"/>, which is what a consumer resolving
        /// <see cref="IEnumerable{T}"/> needs, or under its own concrete type when <c>false</c>.
        /// </param>
        /// <param name="filter">
        /// An optional predicate narrowing what is registered, evaluated after the discovered type has already passed the
        /// assignability and <see cref="SkipAutoRegistrationAttribute"/> checks. Only types it accepts are registered.
        /// </param>
        ///
        /// <returns>The <see cref="IServiceCollection"/> instance with matching implementations registered.</returns>
        ///
        /// <remarks>
        /// Registrations are added rather than replaced, so calling this twice over the same assembly registers everything
        /// twice. Interfaces, abstract classes, and types carrying <see cref="SkipAutoRegistrationAttribute"/> are never
        /// registered.
        /// </remarks>
        ///
        /// <author>Almighty-Shogun</author>
        /// <since>1.0.0</since>
        public IServiceCollection RegisterOnInherit<T>(
            Assembly[] assemblies,
            ServiceLifetime serviceLifetime = ServiceLifetime.Singleton,
            bool addType = true,
            Func<Type, bool>? filter = null
        ) where T : class => serviceCollection.InternalRegister<T>(serviceLifetime, addType, filter, assemblies);

        /// <summary>
        /// Performs the discovery and registration every public overload ends up at, once the assemblies have been resolved
        /// to an explicit array.
        /// </summary>
        ///
        /// <typeparam name="T">The base type or interface to match.</typeparam>
        /// <param name="serviceLifetime">The lifetime applied to every registration.</param>
        /// <param name="addType">
        /// Whether to register each implementation under <typeparamref name="T"/> instead of its own concrete type.
        /// </param>
        /// <param name="filter">An optional predicate applied to each discovered type. Only types it accepts are registered.</param>
        /// <param name="assemblies">
        /// The assemblies to scan. Already resolved by the caller, so an empty array here scans nothing rather than falling
        /// back to the calling assembly.
        /// </param>
        ///
        /// <returns>The <see cref="IServiceCollection"/> instance with matching implementations registered.</returns>
        ///
        /// <author>Almighty-Shogun</author>
        /// <since>1.0.0</since>
        private IServiceCollection InternalRegister<T>(
            ServiceLifetime serviceLifetime,
            bool addType,
            Func<Type, bool>? filter,
            Assembly[] assemblies
        ) where T : class
        {
            IEnumerable<Type> types = TypeDiscovery.FindAssignableTypes<T>(assemblies)
                .Where(t => !t.IsDefined(typeof(SkipAutoRegistrationAttribute), false))
                .Where(t => filter is null || filter(t));

            foreach (Type type in types)
            {
                Type serviceType = addType ? typeof(T) : type;
                serviceCollection.Add(new ServiceDescriptor(serviceType, type, serviceLifetime));
            }

            return serviceCollection;
        }
    }
}
