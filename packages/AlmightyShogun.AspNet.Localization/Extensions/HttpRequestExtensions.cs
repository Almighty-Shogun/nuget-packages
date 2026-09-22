using Microsoft.AspNetCore.Http;
using Microsoft.Net.Http.Headers;

namespace AlmightyShogun.AspNet.Localization;

/// <summary>
/// Provides extensions for reading language preferences from HTTP requests.
/// </summary>
///
/// <author>Almighty-Shogun</author>
/// <since>4.0.0</since>
public static class HttpRequestExtensions
{
    /// <summary>
    /// Provides language preference extensions for an <see cref="HttpRequest"/>.
    /// </summary>
    ///
    /// <param name="httpRequest">The HTTP request.</param>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>4.0.0</since>
    extension(HttpRequest httpRequest)
    {
        /// <summary>
        /// Gets the accepted languages from the <c>Accept-Language</c> header in preference order.
        /// </summary>
        ///
        /// <returns>The accepted languages, or an empty list if none are available.</returns>
        ///
        /// <remarks>
        /// Wildcards, refused languages, malformed language tags, and duplicate languages are excluded.
        /// </remarks>
        ///
        /// <author>Almighty-Shogun</author>
        /// <since>4.0.0</since>
        public IReadOnlyList<string> GetAcceptLanguages()
        {
            IList<StringWithQualityHeaderValue> languages = httpRequest.GetTypedHeaders().AcceptLanguage;

            return
            [
                .. languages
                    .Where(x => x.Value.HasValue && x.Value != "*" && x.Quality.GetValueOrDefault(1) > 0)
                    .OrderByDescending(x => x.Quality.GetValueOrDefault(1))
                    .Select(x => x.Value.ToString())
                    .Where(LanguageTag.IsValid)
                    .Distinct(StringComparer.OrdinalIgnoreCase)
            ];
        }

        /// <summary>
        /// Gets the most preferred language from the <c>Accept-Language</c> header.
        /// </summary>
        ///
        /// <returns>The most preferred language, or <c>null</c> if none are available.</returns>
        ///
        /// <author>Almighty-Shogun</author>
        /// <since>4.0.0</since>
        public string? GetAcceptLanguage()
        {
            IReadOnlyList<string> languages = httpRequest.GetAcceptLanguages();

            return languages.Count > 0 ? languages[0] : null;
        }
    }
}
