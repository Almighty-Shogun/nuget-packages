using Microsoft.AspNetCore.Http;

namespace AlmightyShogun.AspNet.JwtAuth;

/// <summary>
/// Reads this package's authentication cookies off a request, so endpoint code never spells a cookie name or decides for
/// itself what a missing one means.
/// </summary>
///
/// <author>Almighty-Shogun</author>
/// <since>2.3.0</since>
public static class HttpRequestExtensions
{
    /// <summary>
    /// Provides the cookie readers on any request, in both the strict and the tolerant form.
    /// </summary>
    ///
    /// <param name="httpRequest">The request to read from, normally the one currently being served.</param>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>2.3.0</since>
    extension(HttpRequest httpRequest)
    {
        /// <summary>
        /// Reads the refresh-token cookie without failing when it is absent, for a path that treats a signed-out caller
        /// as ordinary rather than exceptional, such as an optional session refresh.
        /// </summary>
        ///
        /// <returns>The cookie value, or <c>null</c> when the request carries no such cookie.</returns>
        ///
        /// <author>Almighty-Shogun</author>
        /// <since>2.3.0</since>
        public string? TryGetRefreshTokenCookie() => httpRequest.Cookies[CookieNames.RefreshToken] ?? null;

        /// <summary>
        /// Reads the refresh-token cookie and fails when it is absent, for a path that cannot proceed without one, such
        /// as a refresh or a sign-out.
        /// </summary>
        ///
        /// <returns>The cookie value, never blank.</returns>
        ///
        /// <exception cref="MissingRefreshTokenException">
        /// The request carries no refresh-token cookie, or carries one that is empty, which is indistinguishable from
        /// being signed out.
        /// </exception>
        ///
        /// <author>Almighty-Shogun</author>
        /// <since>Unreleased</since>
        public string GetRefreshTokenCookie()
        {
            string? refreshToken = httpRequest.Cookies[CookieNames.RefreshToken];

            return string.IsNullOrWhiteSpace(refreshToken)
                ? throw new MissingRefreshTokenException()
                : refreshToken;
        }
    }
}
