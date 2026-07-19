using AlmightyShogun.AspNet.Utils;

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
    /// Attempts to resolve the authentication app for the current request.
    /// </summary>
    ///
    /// <param name="app">The resolved app, or <c>null</c> when app scoping is disabled.</param>
    ///
    /// <returns><c>true</c> when app scoping is disabled or the current request maps to a configured app; otherwise, <c>false</c>.</returns>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    bool TryResolve(out string? app);

    /// <summary>
    /// Resolves the authentication app for the current request.
    /// </summary>
    ///
    /// <exception cref="HttpErrorException">Thrown with status code <c>403</c> when app scoping is active and the current request cannot be resolved.</exception>
    ///
    /// <returns>The resolved app, or <c>null</c> when app scoping is disabled.</returns>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    string? Resolve();

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
    /// <exception cref="HttpErrorException">Thrown with status code <c>403</c> when the host is missing or does not map to a configured application.</exception>
    ///
    /// <returns>The application audience name associated with the host.</returns>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>2.3.0</since>
    string ResolveAppFromHost(string? host);
}
