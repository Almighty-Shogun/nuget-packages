using Microsoft.Extensions.DependencyInjection;

namespace AlmightyShogun.Utils;

/// <summary>
/// Defines a service registration module that can add its own services to a collection.
/// </summary>
///
/// <author>Almighty-Shogun</author>
/// <since>Unreleased</since>
public interface IServiceRegistry
{
    /// <summary>
    /// Configures the services owned by the registry.
    /// </summary>
    ///
    /// <param name="serviceCollection">The service collection to add registrations to.</param>
    ///
    /// <returns>The <see cref="IServiceCollection"/> instance with the registry's services configured.</returns>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    IServiceCollection ConfigureService(IServiceCollection serviceCollection);
}
