using Microsoft.AspNetCore.Http;

namespace AlmightyShogun.AspNet.JwtAuth.Extensions;

public static class HttpRequestExtensions
{
    /// <param name="httpRequest">The <see cref="HttpRequest"/> used to register the functionalities.</param>
    extension(HttpRequest httpRequest)
    {
        /// <summary>
        /// Returns the <see cref="CookieNames.RefreshToken"/> cookie from the <see cref="HttpRequest"/>
        /// </summary>
        /// 
        /// <returns>The value of the <see cref="CookieNames.RefreshToken"/> cookie, <c>null</c> otherwise.</returns>
        ///
        /// <author>Almighty-Shogun</author>
        /// <since>2.3.0</since>
        public string? GetRefreshTokenCookie() => httpRequest.Cookies[CookieNames.RefreshToken] ?? null;
    }
}
