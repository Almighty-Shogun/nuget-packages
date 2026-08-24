using Microsoft.AspNetCore.Http;

namespace AlmightyShogun.AspNet.Localization;

/// <summary>
/// Reports the language a response body was written in. The value travels in a response header, so nothing here takes
/// effect once the response has started.
/// </summary>
///
/// <author>Almighty-Shogun</author>
/// <since>Unreleased</since>
public static class HttpResponseExtensions
{
    /// <summary>
    /// Provides the content-language helpers as extensions on the response.
    /// </summary>
    ///
    /// <param name="httpResponse">
    /// The response being built. It is not written to or completed here, so the body remains the caller's to produce.
    /// </param>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    extension(HttpResponse httpResponse)
    {
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
        /// Sets the response <c>Content-Language</c> header, replacing any language already set, while the response can
        /// still take one and the value is one a header can carry.
        /// </summary>
        ///
        /// <param name="language">The language tag to report the body as being written in.</param>
        ///
        /// <returns>
        /// <c>true</c> when the header was written, or <c>false</c> when the response had already started, or the value
        /// was blank or carried a control character. Nothing is left half-written either way, so a caller with no
        /// alternative language to fall back to can ignore the result.
        /// </returns>
        ///
        /// <remarks>
        /// Reporting the refusal rather than throwing, as a direct header assignment would, is what makes it safe to
        /// call late. Call it from an <c>OnStarting</c> callback when the language is only settled after the body is.
        /// The value is checked rather than trusted, so a tag taken from user input cannot turn a header assignment
        /// into an exception raised while the response is being sent.
        /// </remarks>
        ///
        /// <author>Almighty-Shogun</author>
        /// <since>Unreleased</since>
        public bool TrySetContentLanguage(string language)
        {
            if (httpResponse.HasStarted || string.IsNullOrWhiteSpace(language) || language.Any(char.IsControl))
                return false;

            httpResponse.Headers.ContentLanguage = language;

            return true;
        }
    }
}
