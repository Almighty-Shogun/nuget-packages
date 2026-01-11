using Microsoft.Extensions.DependencyInjection;

namespace AlmightyShogun.Utils;

public interface IService
{
    /// <summary>
    /// Configures necessary services and adds them to the given IServiceCollection.
    /// </summary>
    /// 
    /// <param name="serviceCollection">The IServiceCollection instance to which the services will be added.</param>
    /// 
    /// <returns>The updated IServiceCollection containing the configured services.</returns>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>1.0.0</since>
    IServiceCollection ConfigureService(IServiceCollection serviceCollection);
}
