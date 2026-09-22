using Microsoft.AspNetCore.Http;

namespace AlmightyShogun.AspNet.Localization;

/// <summary>
/// Provides extensions for accessing the response <c>Content-Language</c> header.
/// </summary>
///
/// <author>Almighty-Shogun</author>
/// <since>4.0.0</since>
public static class HttpResponseExtensions
{
    /// <summary>
    /// Provides content language extensions for an <see cref="HttpResponse"/>.
    /// </summary>
    ///
    /// <param name="httpResponse">The HTTP response.</param>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>4.0.0</since>
    extension(HttpResponse httpResponse)
    {
        /// <summary>
        /// Gets the response <c>Content-Language</c> header.
        /// </summary>
        ///
        /// <returns>
        /// The header value, or <c>null</c> if it is not set.
        /// </returns>
        ///
        /// <author>Almighty-Shogun</author>
        /// <since>4.0.0</since>
        public string? GetContentLanguage()
        {
            var language = httpResponse.Headers.ContentLanguage.ToString();

            return string.IsNullOrWhiteSpace(language) ? null : language;
        }

        /// <summary>
        /// Attempts to set the response <c>Content-Language</c> header.
        /// </summary>
        ///
        /// <param name="language">The content language.</param>
        ///
        /// <returns>
        /// <c>true</c> if the header was set; otherwise, <c>false</c>.
        /// </returns>
        ///
        /// <remarks>
        /// The header is not set after the response has started or when
        /// <paramref name="language"/> is blank or contains control characters.
        /// </remarks>
        ///
        /// <author>Almighty-Shogun</author>
        /// <since>4.0.0</since>
        public bool TrySetContentLanguage(string language)
        {
            if (httpResponse.HasStarted || string.IsNullOrWhiteSpace(language) || language.Any(char.IsControl))
                return false;

            httpResponse.Headers.ContentLanguage = language;

            return true;
        }
    }
}
