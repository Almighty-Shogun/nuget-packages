using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.DependencyInjection;

namespace AlmightyShogun.AspNet.Auth;

/// <summary>
/// Writes and deletes this package's refresh-token cookie, so endpoint code never spells a cookie name or picks the
/// attributes it is written and removed with.
/// </summary>
///
/// <author>Almighty-Shogun</author>
/// <since>2.3.0</since>
public static class HttpResponseExtensions
{
    /// <summary>
    /// Provides the refresh-token cookie writers on any response, both applying the same path, <c>SameSite</c> mode, and
    /// secure flag, so the cookie is deleted with the attributes it was written with. The secure flag comes from the
    /// request scheme rather than from the mode, so a <see cref="SameSiteMode.None"/> cookie is marked secure only when
    /// the request that wrote it was.
    /// </summary>
    ///
    /// <param name="httpResponse">
    /// The response the cookie is written on. Its request supplies both the service scope the <c>Auth</c> settings are
    /// read from and the scheme that decides the secure flag.
    /// </param>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>2.3.0</since>
    extension(HttpResponse httpResponse)
    {
        /// <summary>
        /// Writes an <c>HttpOnly</c> refresh token cookie, with the <c>SameSite</c> mode from the bound
        /// <c>AuthSettings</c> and the secure flag taken from the current request scheme. On an application that never
        /// called <c>AddAuth</c> nothing bound that section, so the mode is the record's own default rather than a
        /// configured one.
        /// </summary>
        ///
        /// <param name="token">The refresh token value to store in the cookie.</param>
        /// <param name="days">
        /// The number of days before the cookie expires. Pass <see cref="AuthSettings.RefreshTokenDays"/> to give the
        /// cookie the same lifetime the refresh token itself is configured for; nothing pairs them for you.
        /// </param>
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
        /// Deletes this package's refresh-token cookie, repeating the path it was written with, which is what identifies
        /// the cookie to remove, along with the <c>SameSite</c> mode and secure flag. The secure flag comes from the
        /// current request scheme, so a deletion issued over plain HTTP is not marked secure whatever the mode is.
        /// </summary>
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
    /// <returns>The <c>SameSite</c> mode from the bound <c>AuthSettings</c>, or its default when nothing bound it.</returns>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>4.0.0</since>
    private static SameSiteMode ResolveSameSite(HttpResponse httpResponse)
        => httpResponse.HttpContext.RequestServices.GetRequiredService<IOptions<AuthSettings>>().Value.SameSite;
}
