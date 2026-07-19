using Microsoft.AspNetCore.Http;

namespace AlmightyShogun.AspNet.Utils;

/// <summary>
/// Provides helpers for reading values from HTTP requests.
/// </summary>
///
/// <author>Almighty-Shogun</author>
/// <since>Unreleased</since>
public static class HttpRequestExtensions
{
    /// <summary>
    /// Provides HTTP request extension methods for the target request instance.
    /// </summary>
    ///
    /// <param name="httpRequest">The HTTP request used by the extension methods.</param>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    extension(HttpRequest httpRequest)
    {
        /// <summary>
        /// Gets the preferred language from the request <c>Accept-Language</c> header.
        /// </summary>
        ///
        /// <returns>The preferred language when available; otherwise, <c>null</c>.</returns>
        ///
        /// <author>Almighty-Shogun</author>
        /// <since>Unreleased</since>
        public string? GetAcceptLanguage()
        {
            string header = httpRequest.Headers.AcceptLanguage.ToString();

            if (string.IsNullOrWhiteSpace(header))
            {
                return null;
            }

            string language = header.Split(',', 2)[0].Split(';', 2)[0].Trim();

            return string.IsNullOrWhiteSpace(language) ? null : language;
        }
    }
}
