namespace AlmightyShogun.AspNet.JwtAuth;

/// <summary>
/// Resolves which configured application a request belongs to, from its host. Every token carries an audience and it is
/// always validated, so this is what decides whether a request is even eligible to be authenticated.
/// </summary>
///
/// <author>Almighty-Shogun</author>
/// <since>2.3.0</since>
public interface IAppHostResolver
{
    /// <summary>
    /// Resolves the app for the current request without throwing, for code that wants to decide for itself what an
    /// unknown host means. The result is cached on the request, so repeated calls cost one lookup.
    /// </summary>
    ///
    /// <param name="app">
    /// The resolved app, or <c>null</c> when scoping is off. A <c>null</c> app with a <c>true</c> result means scoping is
    /// disabled rather than that resolution failed.
    /// </param>
    ///
    /// <returns>
    /// <c>true</c> when scoping is off or the host maps to a configured app; <c>false</c> when scoping is on and it does
    /// not, which includes there being no request in flight at all.
    /// </returns>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    bool TryResolve(out string? app);

    /// <summary>
    /// Resolves the app for the current request, refusing rather than guessing when the host is unknown.
    /// </summary>
    ///
    /// <returns>The resolved app, or <c>null</c> when scoping is off and there is nothing to resolve.</returns>
    ///
    /// <exception cref="UnknownAppException">
    /// Scoping is active and the request cannot be attributed: either the host maps to no configured application, or
    /// there is no request in flight at all, as when a background job mints a token. The second carries a <c>null</c>
    /// host rather than the one that failed.
    /// </exception>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    string? Resolve();

    /// <summary>
    /// Resolves a host supplied by the caller rather than the current request, for code resolving a host it already
    /// holds, such as a background job acting on behalf of a tenant.
    /// </summary>
    ///
    /// <param name="host">The host to resolve, matched case-insensitively and falling back to the localhost app.</param>
    /// <param name="app">The audience when the host is known; otherwise an empty string rather than <c>null</c>.</param>
    ///
    /// <returns><c>true</c> when the host maps to an application audience name; otherwise <c>false</c>.</returns>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>2.3.0</since>
    bool TryResolveAppFromHost(string? host, out string app);

    /// <summary>
    /// Resolves a caller-supplied host, refusing rather than guessing when it is unknown.
    /// </summary>
    ///
    /// <param name="host">The host to resolve, matched case-insensitively and falling back to the localhost app.</param>
    ///
    /// <returns>The audience the host maps to.</returns>
    ///
    /// <exception cref="UnknownAppException">
    /// The host is blank, or maps to no configured application and is not a localhost value, carrying the host it failed on.
    /// </exception>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>2.3.0</since>
    string ResolveAppFromHost(string? host);
}
