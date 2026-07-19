using Microsoft.AspNetCore.Http;
using AlmightyShogun.AspNet.Utils;

namespace AlmightyShogun.AspNet.JwtAuth;

/// <summary>
/// Provides helpers for reading authentication cookies from HTTP requests.
/// </summary>
///
/// <author>Almighty-Shogun</author>
/// <since>2.3.0</since>
public static class HttpRequestExtensions
{
    /// <summary>
    /// Provides request extension methods for reading JWT auth cookies.
    /// </summary>
    ///
    /// <param name="httpRequest">The HTTP request used by the extension methods.</param>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>2.3.0</since>
    extension(HttpRequest httpRequest)
    {
        /// <summary>
        /// Tries to return the configured refresh token cookie from the request.
        /// </summary>
        ///
        /// <returns>The refresh token cookie value, or <c>null</c> when the cookie is not present.</returns>
        ///
        /// <author>Almighty-Shogun</author>
        /// <since>2.3.0</since>
        public string? TryGetRefreshTokenCookie() => httpRequest.Cookies[CookieNames.RefreshToken] ?? null;

        /// <summary>
        /// Returns the configured refresh token cookie from the request and fails when it is missing.
        /// </summary>
        ///
        /// <exception cref="HttpErrorException">Thrown with status code <c>401</c> when the refresh-token cookie is missing or empty.</exception>
        ///
        /// <returns>The refresh token cookie value.</returns>
        ///
        /// <author>Almighty-Shogun</author>
        /// <since>Unreleased</since>
        public string GetRefreshTokenCookie()
        {
            string? refreshToken = httpRequest.Cookies[CookieNames.RefreshToken];

            return string.IsNullOrWhiteSpace(refreshToken)
                ? throw new HttpErrorException(StatusCodes.Status401Unauthorized)
                : refreshToken;
        }
    }
}
