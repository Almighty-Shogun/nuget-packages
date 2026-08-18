using System.Globalization;
using Microsoft.AspNetCore.Http;
using System.Text.RegularExpressions;

namespace AlmightyShogun.AspNet.Utils;

/// <summary>
/// Reads content negotiation off the request. Every language returned has been matched against a language tag pattern
/// first, because these values reach the filesystem when message files are resolved.
/// </summary>
///
/// <author>Almighty-Shogun</author>
/// <since>Unreleased</since>
public static partial class HttpRequestExtensions
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
        /// assigned, highest first. Entries that are not well-formed language tags are dropped, and duplicates are
        /// removed so the first occurrence keeps its position.
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
            var header = httpRequest.Headers.AcceptLanguage.ToString();

            if (string.IsNullOrWhiteSpace(header))
                return [];

            List<(string Language, double Quality, int Position)> candidates = [];

            foreach (string entry in header.Split(',', StringSplitOptions.RemoveEmptyEntries))
            {
                string[] parts = entry.Split(';', StringSplitOptions.RemoveEmptyEntries);
                string language = parts[0].Trim();

                if (language == "*" || !LanguageTagRegex().IsMatch(language)) continue;

                candidates.Add((language, ParseQuality(parts), candidates.Count));
            }

            return
            [
                .. candidates
                    .OrderByDescending(candidate => candidate.Quality)
                    .ThenBy(candidate => candidate.Position)
                    .Select(candidate => candidate.Language)
                    .Distinct(StringComparer.OrdinalIgnoreCase)
            ];
        }

        /// <summary>
        /// Gets the first language listed in the request <c>Accept-Language</c> header, ignoring quality values. Reach
        /// for <see cref="GetAcceptLanguages"/> when a lower-ranked language is worth trying before the default.
        /// </summary>
        ///
        /// <returns>
        /// The first listed language, or <c>null</c> when the header is absent or its first entry is not a well-formed
        /// language tag, including when that entry is the <c>*</c> wildcard.
        /// </returns>
        ///
        /// <author>Almighty-Shogun</author>
        /// <since>Unreleased</since>
        public string? GetAcceptLanguage()
        {
            var header = httpRequest.Headers.AcceptLanguage.ToString();

            if (string.IsNullOrWhiteSpace(header))
                return null;

            string language = header.Split(',', 2)[0].Split(';', 2)[0].Trim();

            return LanguageTagRegex().IsMatch(language) ? language : null;
        }
    }

    /// <summary>
    /// Reads the <c>q</c> parameter from an <c>Accept-Language</c> entry.
    /// </summary>
    ///
    /// <param name="parts">
    /// The semicolon-separated parts of a single header entry, the language itself first. Only the parts after it are
    /// examined, and the first one starting with <c>q=</c> wins.
    /// </param>
    ///
    /// <returns>The quality value, or <c>1</c> when it is absent or unparseable, which is what the specification says.</returns>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    private static double ParseQuality(string[] parts)
    {
        foreach (string part in parts.Skip(1))
        {
            string trimmed = part.Trim();

            if (!trimmed.StartsWith("q=", StringComparison.OrdinalIgnoreCase))
                continue;

            return double.TryParse(trimmed[2..], NumberStyles.Float, CultureInfo.InvariantCulture, out double quality)
                ? quality
                : 1;
        }

        return 1;
    }

    /// <summary>
    /// Matches a well-formed language tag. Anything else is rejected, because the value reaches the filesystem when
    /// message files are resolved and an unvalidated header would allow directory traversal.
    /// </summary>
    ///
    /// <returns>
    /// The generated matcher for a two or three letter primary tag followed by any number of alphanumeric subtags. It
    /// is deliberately narrower than the grammar in the specification, since anything broader admits a path separator.
    /// </returns>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    [GeneratedRegex("^[A-Za-z]{2,3}(-[A-Za-z0-9]{2,8})*$")]
    private static partial Regex LanguageTagRegex();
}
