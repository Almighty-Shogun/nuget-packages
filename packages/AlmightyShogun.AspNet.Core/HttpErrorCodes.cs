using System.Text;
using Microsoft.AspNetCore.WebUtilities;

namespace AlmightyShogun.AspNet.Core;

/// <summary>
/// Maps a status code to the snake-case identifier clients branch on, derived from the reason phrase the framework
/// names the status with. These strings are part of the response contract, so a mapping must not change once released,
/// even to correct its wording.
/// </summary>
///
/// <remarks>
/// Deriving from <see cref="ReasonPhrases"/> rather than listing every status means a status the framework learns to
/// name is covered without an edit here. It also ties the contract to a table this package does not own, which is why
/// <c>425</c> is pinned below: the derived form is only as stable as the phrase it comes from.
/// </remarks>
///
/// <author>Almighty-Shogun</author>
/// <since>Unreleased</since>
internal static class HttpErrorCodes
{
    /// <summary>
    /// The status the reason phrase table does not name, kept because clients already branch on the identifier it
    /// mapped to before the table became the source.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    private const int _tooEarly = 425;

    /// <summary>
    /// Looks up the identifier a client sees in the <c>error</c> field, for a status the application is about to return.
    /// </summary>
    ///
    /// <param name="statusCode">
    /// The status code being returned. Every status the framework names is mapped, not only error statuses, so a
    /// non-error code passed here yields its own phrase rather than being rejected.
    /// </param>
    ///
    /// <returns>
    /// The identifier for a status the framework names, such as <c>not_found</c>, or <c>http_error_{code}</c> for one
    /// it does not. The fallback keeps the field populated, so a client can always read a code.
    /// </returns>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    internal static string FromStatusCode(int statusCode)
    {
        if (statusCode is _tooEarly) return "too_early";

        string phrase = ReasonPhrases.GetReasonPhrase(statusCode);

        return string.IsNullOrEmpty(phrase) ? $"http_error_{statusCode}" : ToSnakeCase(phrase);
    }

    /// <summary>
    /// Reduces a reason phrase to the lowercase underscore-separated form the <c>error</c> field carries.
    /// </summary>
    ///
    /// <param name="phrase">The reason phrase as the framework spells it, such as <c>Range Not Satisfiable</c>.</param>
    ///
    /// <returns>The phrase with its words joined by underscores, such as <c>range_not_satisfiable</c>.</returns>
    ///
    /// <remarks>
    /// Punctuation is dropped rather than replaced, which is what turns <c>I'm a teapot</c> into <c>im_a_teapot</c>
    /// instead of splitting it at the apostrophe. Only spaces and hyphens become separators.
    /// </remarks>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    private static string ToSnakeCase(string phrase)
    {
        StringBuilder builder = new(phrase.Length);

        foreach (char character in phrase)
            if (char.IsLetterOrDigit(character))
                builder.Append(char.ToLowerInvariant(character));
            else if (character is ' ' or '-')
                builder.Append('_');

        return builder.ToString();
    }
}
