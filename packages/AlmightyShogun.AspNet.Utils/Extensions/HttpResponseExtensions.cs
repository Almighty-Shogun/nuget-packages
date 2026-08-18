using Microsoft.AspNetCore.Http;

namespace AlmightyShogun.AspNet.Utils;

/// <summary>
/// Writes response headers and cookies. Everything here goes through the response headers, so nothing takes effect once
/// the response has started.
/// </summary>
///
/// <author>Almighty-Shogun</author>
/// <since>2.3.0</since>
public static class HttpResponseExtensions
{
    /// <summary>
    /// Provides the header and cookie helpers as extensions on the response.
    /// </summary>
    ///
    /// <param name="httpResponse">
    /// The response being built. It is not written to or completed here, so the body remains the caller's to produce.
    /// </param>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>2.3.0</since>
    extension(HttpResponse httpResponse)
    {
        /// <summary>
        /// Deletes the named cookies by emitting an expired <c>Set-Cookie</c> for each.
        /// </summary>
        ///
        /// <param name="cookieNames">
        /// The cookie names to delete. Blank names are ignored, so a name read from configuration can be passed without
        /// a guard.
        /// </param>
        ///
        /// <remarks>
        /// The expiry is scoped to the root path and the current host. A cookie written with a different path or domain
        /// is a different cookie to the browser and survives this call; delete it through <c>Cookies.Delete</c> with
        /// matching options instead.
        /// </remarks>
        ///
        /// <author>Almighty-Shogun</author>
        /// <since>2.3.0</since>
        public void DeleteCookies(params string[] cookieNames)
        {
            foreach (string cookieName in cookieNames.Where(cookieName => !string.IsNullOrWhiteSpace(cookieName)))
                httpResponse.Cookies.Delete(cookieName);
        }

        /// <summary>
        /// Gets the response <c>Content-Language</c> header as it currently stands.
        /// </summary>
        ///
        /// <returns>
        /// The header value, or <c>null</c> when it has not been set. Multiple languages come back joined by commas
        /// rather than as separate values.
        /// </returns>
        ///
        /// <author>Almighty-Shogun</author>
        /// <since>Unreleased</since>
        public string? GetContentLanguage()
        {
            var language = httpResponse.Headers.ContentLanguage.ToString();

            return string.IsNullOrWhiteSpace(language) ? null : language;
        }

        /// <summary>
        /// Sets the response <c>Content-Language</c> header, replacing any language already set.
        /// </summary>
        ///
        /// <param name="language">The language tag to report the body as being written in.</param>
        ///
        /// <remarks>
        /// Does nothing once the response has started, rather than throwing as a direct header assignment would. Call it
        /// from an <c>OnStarting</c> callback when the language is only known after the body has been produced.
        /// </remarks>
        ///
        /// <author>Almighty-Shogun</author>
        /// <since>Unreleased</since>
        public void SetContentLanguage(string language)
        {
            if (!httpResponse.HasStarted)
                httpResponse.Headers.ContentLanguage = language;
        }
    }
}
