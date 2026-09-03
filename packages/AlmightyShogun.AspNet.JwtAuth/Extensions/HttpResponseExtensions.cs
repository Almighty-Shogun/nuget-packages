using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.DependencyInjection;

namespace AlmightyShogun.AspNet.JwtAuth;

/// <summary>
/// Provides helpers for writing and clearing authentication cookies on HTTP responses.
/// </summary>
///
/// <author>Almighty-Shogun</author>
/// <since>2.3.0</since>
public static class HttpResponseExtensions
{
    /// <summary>
    /// Provides response extension methods for writing and clearing JWT auth cookies.
    /// </summary>
    ///
    /// <param name="httpResponse">The HTTP response used by the extension methods.</param>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>2.3.0</since>
    extension(HttpResponse httpResponse)
    {
        /// <summary>
        /// Writes an <c>HttpOnly</c> refresh token cookie, with the <c>SameSite</c> mode configured under
        /// <c>Auth:SameSite</c> and the secure flag taken from the current request scheme.
        /// </summary>
        ///
        /// <param name="token">The refresh token value to store in the cookie.</param>
        /// <param name="days">The number of days before the cookie expires.</param>
        ///
        /// <exception cref="InvalidOperationException">
        /// The request has no <see cref="AuthSettings"/> registration to read the mode from, which means
        /// <c>AddJwtAuth</c> was never called on the application's services.
        /// </exception>
        ///
        /// <author>Almighty-Shogun</author>
        /// <since>2.3.0</since>
        public void SetRefreshTokenCookie(string token, int days)
            => httpResponse.Cookies.Append(CookieNames.RefreshToken, token, new CookieOptions
            {
                Path = "/",
                HttpOnly = true,
                SameSite = ResolveSameSite(httpResponse),
                Expires = DateTimeOffset.UtcNow.AddDays(days),
                Secure = httpResponse.HttpContext.Request.IsHttps
            });

        /// <summary>
        /// Deletes the default authentication cookies used by the package, repeating the path, <c>SameSite</c> mode, and
        /// secure flag they were written with, since a browser ignores a deletion whose attributes do not match.
        /// </summary>
        ///
        /// <exception cref="InvalidOperationException">
        /// The request has no <see cref="AuthSettings"/> registration to read the mode from, which means
        /// <c>AddJwtAuth</c> was never called on the application's services.
        /// </exception>
        ///
        /// <author>Almighty-Shogun</author>
        /// <since>2.3.0</since>
        public void DeleteAuthCookies() => httpResponse.Cookies.Delete(CookieNames.RefreshToken, new CookieOptions
        {
            Path = "/",
            SameSite = ResolveSameSite(httpResponse),
            Secure = httpResponse.HttpContext.Request.IsHttps
        });
    }

    /// <summary>
    /// Reads the configured cookie mode from the request's own service scope, which is what keeps the extension methods
    /// callable as plain response helpers instead of forcing every caller to inject the settings and pass them in.
    /// </summary>
    ///
    /// <param name="httpResponse">The response being written to, used for the service provider of its request.</param>
    ///
    /// <returns>The <c>SameSite</c> mode configured under <c>Auth:SameSite</c>.</returns>
    ///
    /// <exception cref="InvalidOperationException">
    /// No <see cref="AuthSettings"/> options are registered, so <c>AddJwtAuth</c> was never called.
    /// </exception>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    private static SameSiteMode ResolveSameSite(HttpResponse httpResponse)
        => httpResponse.HttpContext.RequestServices.GetRequiredService<IOptions<AuthSettings>>().Value.SameSite;
}
