using Microsoft.Extensions.DependencyInjection;

namespace AlmightyShogun.Utils;

/// <summary>
/// Defines a module for registering services with the specified service collection.
/// </summary>
///
/// <author>Almighty-Shogun</author>
/// <since>1.0.0</since>
public interface IServiceRegistry
{
    /// <summary>
    /// Registers services with the specified service collection.
    /// </summary>
    ///
    /// <param name="serviceCollection">
    /// The service collection to configure.
    /// </param>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>4.0.0</since>
    void ConfigureService(IServiceCollection serviceCollection);
}
