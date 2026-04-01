namespace AlmightyShogun.AspNet.JwtAuth;

public interface IAppHostResolver
{
    /// <summary>
    /// Attempts to resolve an application name from the provided host.
    /// </summary>
    ///
    /// <param name="host">The host to resolve the application name from.</param>
    /// <param name="app">The application name of the host.</param>
    /// 
    /// <returns><c>true</c> if the app name can be found in the configured, <c>false</c> otherwise.</returns>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>2.3.0</since>
    bool TryResolveAppFromHost(string? host, out string app);

    /// <summary>
    /// Resolves an application name from the provided host or throws when the host is unknown.
    /// </summary>
    ///
    /// <param name="host">The host to resolve the application name from.</param>
    /// 
    /// <returns>The application name of the host.</returns>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>2.3.0</since>
    string ResolveAppFromHost(string? host);
}
