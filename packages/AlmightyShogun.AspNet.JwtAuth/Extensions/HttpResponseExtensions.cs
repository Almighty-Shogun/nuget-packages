using Microsoft.AspNetCore.Http;

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
        /// Writes an <c>HttpOnly</c> refresh token cookie.
        /// </summary>
        ///
        /// <param name="token">The refresh token value to store in the cookie.</param>
        /// <param name="days">The number of days before the cookie expires.</param>
        ///
        /// <author>Almighty-Shogun</author>
        /// <since>2.3.0</since>
        public void SetRefreshTokenCookie(string token, int days) => httpResponse.Cookies.Append(CookieNames.RefreshToken, token, new CookieOptions
        {
            Path = "/",
            HttpOnly = true,
            SameSite = SameSiteMode.Lax,
            Expires = DateTimeOffset.UtcNow.AddDays(days),
            Secure = httpResponse.HttpContext.Request.IsHttps
        });

        /// <summary>
        /// Deletes the default authentication cookies used by the package.
        /// </summary>
        ///
        /// <author>Almighty-Shogun</author>
        /// <since>2.3.0</since>
        public void DeleteAuthCookies()
        {
            httpResponse.Cookies.Delete(CookieNames.RefreshToken, new CookieOptions
            {
                Path = "/",
                SameSite = SameSiteMode.Lax,
                Secure = httpResponse.HttpContext.Request.IsHttps
            });
        }
    }
}
