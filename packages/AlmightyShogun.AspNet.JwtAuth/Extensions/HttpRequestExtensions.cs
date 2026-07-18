using Microsoft.AspNetCore.Http;

namespace AlmightyShogun.AspNet.JwtAuth;

/// <summary>
/// Provides helpers for reading authentication cookies from HTTP requests.
/// </summary>
///
/// <author>Almighty-Shogun</author>
/// <since>2.3.0</since>
public static class HttpRequestExtensions
{
    /// <param name="httpRequest">The <see cref="HttpRequest"/> used to register the functionalities.</param>
    extension(HttpRequest httpRequest)
    {
        /// <summary>
        /// Returns the configured refresh token cookie from the request.
        /// </summary>
        ///
        /// <returns>The refresh token cookie value, or <c>null</c> when the cookie is not present.</returns>
        ///
        /// <author>Almighty-Shogun</author>
        /// <since>2.3.0</since>
        public string? GetRefreshTokenCookie() => httpRequest.Cookies[CookieNames.RefreshToken] ?? null;
    }
}
