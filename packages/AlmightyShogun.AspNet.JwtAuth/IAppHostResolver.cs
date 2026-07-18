namespace AlmightyShogun.AspNet.JwtAuth;

/// <summary>
/// Resolves the application audience name associated with an incoming request host.
/// </summary>
///
/// <author>Almighty-Shogun</author>
/// <since>2.3.0</since>
public interface IAppHostResolver
{
    /// <summary>
    /// Attempts to resolve an application name from the provided host.
    /// </summary>
    ///
    /// <param name="host">The request host name to resolve.</param>
    /// <param name="app">The resolved application audience name when the host is known; otherwise an empty string.</param>
    ///
    /// <returns><c>true</c> when the host maps to an application audience name; otherwise <c>false</c>.</returns>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>2.3.0</since>
    bool TryResolveAppFromHost(string? host, out string app);

    /// <summary>
    /// Resolves an application name from the provided host or throws when the host is unknown.
    /// </summary>
    ///
    /// <param name="host">The request host name to resolve.</param>
    ///
    /// <exception cref="UnauthorizedAccessException">Thrown when the host is missing or does not map to a configured application.</exception>
    ///
    /// <returns>The application audience name associated with the host.</returns>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>2.3.0</since>
    string ResolveAppFromHost(string? host);
}
