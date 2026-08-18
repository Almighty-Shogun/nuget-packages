using Microsoft.Extensions.DependencyInjection;

namespace AlmightyShogun.Utils;

/// <summary>
/// Defines a reusable registration module that groups related service registrations behind a single type, so a feature can
/// own its own wiring instead of spreading it across startup code. An implementation is resolved by
/// <see cref="ServiceCollectionExtensions.AddService{T}"/>, which constructs it directly rather than through the container.
/// </summary>
///
/// <remarks>
/// Because the module is constructed with <c>new()</c> before any provider exists, it cannot take constructor dependencies.
/// Anything it needs must be passed through the service collection it receives, or read from configuration registered on it.
/// </remarks>
///
/// <author>Almighty-Shogun</author>
/// <since>Unreleased</since>
public interface IServiceRegistry
{
    /// <summary>
    /// Adds the module's registrations to the supplied collection. Called once during startup, before the service provider is
    /// built, so the implementation may register, decorate, or configure freely but must not resolve anything.
    /// </summary>
    ///
    /// <param name="serviceCollection">
    /// The collection the module adds its registrations to. It is the application's live collection, not a copy, so every
    /// change is visible to the rest of startup.
    /// </param>
    ///
    /// <returns>
    /// The collection the caller should continue building on. Return <paramref name="serviceCollection"/> unless the module
    /// deliberately substitutes a different collection, because <see cref="ServiceCollectionExtensions.AddService{T}"/>
    /// hands this value straight back to its own caller for chaining.
    /// </returns>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    IServiceCollection ConfigureService(IServiceCollection serviceCollection);
}
