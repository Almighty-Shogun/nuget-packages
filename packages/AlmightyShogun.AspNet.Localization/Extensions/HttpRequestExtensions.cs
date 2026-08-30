using Microsoft.AspNetCore.Http;
using Microsoft.Net.Http.Headers;

namespace AlmightyShogun.AspNet.Localization;

/// <summary>
/// Reads content negotiation off the request. Every language returned has been matched against a language tag pattern
/// first, because these values reach the filesystem when message files are resolved.
/// </summary>
///
/// <author>Almighty-Shogun</author>
/// <since>Unreleased</since>
public static class HttpRequestExtensions
{
    /// <summary>
    /// Provides the negotiation helpers as extensions on the request.
    /// </summary>
    ///
    /// <param name="httpRequest">
    /// The request being served. Only headers are read, so the helpers do not consume the body and can be called before
    /// or after model binding without affecting it.
    /// </param>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    extension(HttpRequest httpRequest)
    {
        /// <summary>
        /// Gets every language from the request <c>Accept-Language</c> header, ordered by the quality value the client
        /// assigned, highest first. Entries that are not well-formed language tags are dropped, as are those the client
        /// refused with <c>q=0</c>, and duplicates are removed so the first occurrence keeps its position.
        /// </summary>
        ///
        /// <returns>
        /// The accepted languages in client preference order, or an empty list when the header is absent, holds only
        /// the <c>*</c> wildcard, or names nothing that parses as a language tag.
        /// </returns>
        ///
        /// <remarks>
        /// The wildcard is dropped rather than expanded, since the package resolves messages against files on disk and
        /// has no list of supported languages to expand it into.
        /// </remarks>
        ///
        /// <author>Almighty-Shogun</author>
        /// <since>Unreleased</since>
        public IReadOnlyList<string> GetAcceptLanguages()
        {
            IList<StringWithQualityHeaderValue>? languages = httpRequest.GetTypedHeaders().AcceptLanguage;

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
        /// Gets the language the client ranked highest, which is the first entry <see cref="GetAcceptLanguages"/>
        /// returns. Reach for that method directly when a lower-ranked language is worth trying before the default.
        /// </summary>
        ///
        /// <returns>
        /// The highest-ranked accepted language, or <c>null</c> when nothing in the header survives negotiation: an
        /// absent header, only the <c>*</c> wildcard, only refusals at <c>q=0</c>, or nothing that parses as a tag.
        /// </returns>
        ///
        /// <author>Almighty-Shogun</author>
        /// <since>Unreleased</since>
        public string? GetAcceptLanguage()
        {
            IReadOnlyList<string> languages = httpRequest.GetAcceptLanguages();

            return languages.Count > 0 ? languages[0] : null;
        }
    }
}
