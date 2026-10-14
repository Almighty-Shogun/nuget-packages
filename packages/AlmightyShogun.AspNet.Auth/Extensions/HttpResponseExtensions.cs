
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.DependencyInjection;

namespace AlmightyShogun.AspNet.Auth;

/// <summary>
/// Provides extensions for managing refresh token cookies.
/// </summary>
/// <author>Almighty-Shogun</author>
/// <since>2.3.0</since>
public static class HttpResponseExtensions
{
    /// <summary>
    /// Provides refresh token cookie extensions for HTTP responses.
    /// </summary>
    /// <param name="httpResponse">The HTTP response.</param>
    /// <author>Almighty-Shogun</author>
    /// <since>2.3.0</since>
    extension(HttpResponse httpResponse)
    {
        /// <summary>
        /// Sets a secure, HttpOnly refresh token cookie using the configured SameSite mode.
        /// </summary>
        /// <param name="token">The refresh token.</param>
        /// <param name="days">The cookie lifetime in days (1–365).</param>
        /// <exception cref="ArgumentOutOfRangeException">
        /// Thrown when the lifetime is outside the allowed range.
        /// </exception>
        /// <author>Almighty-Shogun</author>
        /// <since>2.3.0</since>
        public void SetRefreshTokenCookie(string token, int days)
        {
            ArgumentOutOfRangeException.ThrowIfLessThan(days, 1);
            ArgumentOutOfRangeException.ThrowIfGreaterThan(days, 365);

            httpResponse.Cookies.Append(
                CookieNames.RefreshToken,
                token,
                new CookieOptions
                {
                    Path = "/",
                    HttpOnly = true,
                    SameSite = ResolveSameSite(httpResponse),
                    Expires = DateTimeOffset.UtcNow.AddDays(days),
                    Secure = true
                });
        }

        /// <summary>
        /// Deletes the refresh token cookie using its configured attributes.
        /// </summary>
        /// <author>Almighty-Shogun</author>
        /// <since>2.3.0</since>
        public void DeleteAuthCookies() => httpResponse.Cookies.Delete(CookieNames.RefreshToken, new CookieOptions
        {
            Path = "/",
            SameSite = ResolveSameSite(httpResponse),
            Secure = true
        });
    }

    /// <summary>
    /// Retrieves the configured SameSite mode from request services.
    /// </summary>
    /// <param name="httpResponse">The HTTP response.</param>
    /// <returns>The configured SameSite mode.</returns>
    /// <author>Almighty-Shogun</author>
    /// <since>4.0.0</since>
    private static SameSiteMode ResolveSameSite(HttpResponse httpResponse)
        => httpResponse.HttpContext.RequestServices.GetRequiredService<IOptions<AuthSettings>>().Value.SameSite;
}
