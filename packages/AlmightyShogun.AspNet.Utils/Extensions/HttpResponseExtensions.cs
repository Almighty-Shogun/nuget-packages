using Microsoft.AspNetCore.Http;

namespace AlmightyShogun.AspNet.Utils;

/// <summary>
/// Provides helpers for mutating HTTP responses.
/// </summary>
///
/// <author>Almighty-Shogun</author>
/// <since>2.3.0</since>
public static class HttpResponseExtensions
{
    /// <summary>
    /// Provides HTTP response extension methods for the target response instance.
    /// </summary>
    ///
    /// <param name="httpResponse">The HTTP response used by the extension methods.</param>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>2.3.0</since>
    extension(HttpResponse httpResponse)
    {
        /// <summary>
        /// Deletes the specified cookies.
        /// </summary>
        ///
        /// <param name="cookieNames">The cookie names to delete. Blank names are ignored.</param>
        ///
        /// <author>Almighty-Shogun</author>
        /// <since>2.3.0</since>
        public void DeleteCookies(params string[] cookieNames)
        {
            foreach (string cookieName in cookieNames.Where(cookieName => !string.IsNullOrWhiteSpace(cookieName)))
            {
                httpResponse.Cookies.Delete(cookieName);
            }
        }

        /// <summary>
        /// Gets the response <c>Content-Language</c> header.
        /// </summary>
        ///
        /// <returns>The content language when available; otherwise, <c>null</c>.</returns>
        ///
        /// <author>Almighty-Shogun</author>
        /// <since>Unreleased</since>
        public string? GetContentLanguage()
        {
            string language = httpResponse.Headers.ContentLanguage.ToString();

            return string.IsNullOrWhiteSpace(language) ? null : language;
        }

        /// <summary>
        /// Sets the response <c>Content-Language</c> header when the response has not started.
        /// </summary>
        ///
        /// <param name="language">The content language.</param>
        ///
        /// <author>Almighty-Shogun</author>
        /// <since>Unreleased</since>
        public void SetContentLanguage(string language)
        {
            if (!httpResponse.HasStarted)
            {
                httpResponse.Headers.ContentLanguage = language;
            }
        }
    }
}
