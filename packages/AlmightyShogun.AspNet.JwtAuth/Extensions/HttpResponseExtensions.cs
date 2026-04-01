using Microsoft.AspNetCore.Http;

namespace AlmightyShogun.AspNet.JwtAuth.Extensions;

public static class HttpResponseExtensions
{
    /// <param name="httpResponse">The <see cref="HttpResponse"/> used to register the functionalities.</param>
    extension(HttpResponse httpResponse)
    {
        /// <summary>
        /// Writes an <c>HttpOnly</c> refresh token cookie.
        /// </summary>
        ///
        /// <param name="token">The access token.</param>
        /// <param name="days">The amount of days the cookie is valid.</param>
        ///
        /// <author>Almighty-Shogun</author>
        /// <since>2.3.0</since>
        public void SetRefreshTokenCookie(string token, int days) => httpResponse.Cookies.Append(CookieNames.RefreshToken, token, new CookieOptions
        {
            HttpOnly = true,
            Secure = httpResponse.HttpContext.Request.IsHttps,
            SameSite = SameSiteMode.Strict,
            Path = "/",
            Expires = DateTimeOffset.UtcNow.AddDays(days)
        });

        /// <summary>
        /// Deletes the default auth cookies used by the package.
        /// </summary>
        ///
        /// <author>Almighty-Shogun</author>
        /// <since>2.3.0</since>
        public void DeleteAuthCookies()
        {
            httpResponse.Cookies.Delete(CookieNames.RefreshToken);
        }
    }
}
