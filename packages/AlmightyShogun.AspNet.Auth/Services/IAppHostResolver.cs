namespace AlmightyShogun.AspNet.Auth;

/// <summary>
/// Resolves application audiences from request hosts.
/// </summary>
/// <author>Almighty-Shogun</author>
/// <since>2.3.0</since>
public interface IAppHostResolver
{
    /// <summary>
    /// Attempts to resolve the application for the current request.
    /// </summary>
    /// <param name="app">The resolved application, or null when scoping is disabled.</param>
    /// <returns>Whether resolution succeeded or scoping is disabled.</returns>
    /// <author>Almighty-Shogun</author>
    /// <since>4.0.0</since>
    bool TryResolve(out string? app);

    /// <summary>
    /// Resolves the application for the current request.
    /// </summary>
    /// <returns>The resolved application, or null when scoping is disabled.</returns>
    /// <exception cref="UnknownAppException">
    /// Thrown when scoping is enabled and the application cannot be resolved.
    /// </exception>
    /// <author>Almighty-Shogun</author>
    /// <since>4.0.0</since>
    string? Resolve();

    /// <summary>
    /// Attempts to resolve an application from a specified host.
    /// </summary>
    /// <param name="host">The host to resolve.</param>
    /// <param name="app">The resolved application, or an empty string on failure.</param>
    /// <returns>Whether the host maps to an application.</returns>
    /// <author>Almighty-Shogun</author>
    /// <since>2.3.0</since>
    bool TryResolveAppFromHost(string? host, out string app);

    /// <summary>
    /// Resolves an application from a specified host.
    /// </summary>
    /// <param name="host">The host to resolve.</param>
    /// <returns>The resolved application.</returns>
    /// <exception cref="UnknownAppException">
    /// Thrown when the host cannot be resolved.
    /// </exception>
    /// <author>Almighty-Shogun</author>
    /// <since>2.3.0</since>
    string ResolveAppFromHost(string? host);
}
