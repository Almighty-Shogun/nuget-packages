using System.Text;
using Microsoft.AspNetCore.WebUtilities;

namespace AlmightyShogun.AspNet.Core;

/// <summary>
/// Maps HTTP status codes to machine-readable error codes.
/// </summary>
///
/// <author>Almighty-Shogun</author>
/// <since>4.0.0</since>
internal static class HttpErrorCodes
{
    private const int _tooEarly = 425;

    /// <summary>
    /// Returns the error code for an HTTP status code.
    /// </summary>
    ///
    /// <param name="statusCode">The HTTP status code.</param>
    ///
    /// <returns>The reason phrase in snake case, or <c>http_error_{code}</c> when no reason phrase is known.</returns>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>4.0.0</since>
    internal static string FromStatusCode(int statusCode)
    {
        if (statusCode is _tooEarly) return "too_early";

        string phrase = ReasonPhrases.GetReasonPhrase(statusCode);

        return string.IsNullOrEmpty(phrase) ? $"http_error_{statusCode}" : ToSnakeCase(phrase);
    }

    /// <summary>
    /// Converts a reason phrase to a lowercase snake-case identifier.
    /// </summary>
    ///
    /// <param name="phrase">The reason phrase to convert.</param>
    ///
    /// <returns>The converted identifier.</returns>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>4.0.0</since>
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
