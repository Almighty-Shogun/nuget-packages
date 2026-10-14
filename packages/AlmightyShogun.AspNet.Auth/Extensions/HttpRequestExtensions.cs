using Microsoft.AspNetCore.Http;

namespace AlmightyShogun.AspNet.Auth;

/// <summary>
/// Provides extensions for retrieving authentication cookies.
/// </summary>
/// <author>Almighty-Shogun</author>
/// <since>2.3.0</since>
public static class HttpRequestExtensions
{
    /// <summary>
    /// Provides authentication cookie extensions for HTTP requests.
    /// </summary>
    /// <param name="httpRequest">The HTTP request.</param>
    /// <author>Almighty-Shogun</author>
    /// <since>2.3.0</since>
    extension(HttpRequest httpRequest)
    {
        /// <summary>
        /// Retrieves the refresh token cookie if present.
        /// </summary>
        /// <returns>The refresh token, or null if missing or blank.</returns>
        /// <author>Almighty-Shogun</author>
        /// <since>4.0.0</since>
        public string? TryGetRefreshTokenCookie()
        {
            string? refreshToken = httpRequest.Cookies[CookieNames.RefreshToken];

            return string.IsNullOrWhiteSpace(refreshToken) ? null : refreshToken;
        }

        /// <summary>
        /// Retrieves the refresh token cookie.
        /// </summary>
        /// <returns>The non-blank refresh token.</returns>
        /// <exception cref="MissingRefreshTokenException">
        /// Thrown when the refresh token cookie is missing or blank.
        /// </exception>
        /// <author>Almighty-Shogun</author>
        /// <since>2.3.0</since>
        public string GetRefreshTokenCookie() => httpRequest.TryGetRefreshTokenCookie() ?? throw new MissingRefreshTokenException();
    }
}
