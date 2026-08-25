using System.Text;
using System.Buffers.Binary;
using System.Security.Cryptography;

namespace AlmightyShogun.AspNet.RequestValidation;

/// <summary>
/// One failure as it is held while validating: a message key and its parameters, resolved into text only when the response is built.
/// </summary>
///
/// <param name="Code">The stable numeric validation error code.</param>
/// <param name="Key">The validation message key.</param>
/// <param name="Error">The public validation error identifier.</param>
/// <param name="Parameters">The validation message parameters.</param>
///
/// <author>Almighty-Shogun</author>
/// <since>Unreleased</since>
internal sealed record ValidationError(long Code, string Key, string Error, object?[] Parameters)
{
    /// <summary>
    /// Records a failure by key rather than by text, so the language is chosen when the response is written and not when the rule ran.
    /// </summary>
    ///
    /// <param name="key">The message key the failure reports, resolved into a sentence only when the response is written.</param>
    /// <param name="parameters">The values substituted into the message template by position, empty when the message takes none.</param>
    ///
    /// <returns>The validation error.</returns>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    public static ValidationError From(string key, object?[] parameters) => new(ToNumericCode(key), key, ToErrorName(key), parameters);

    /// <summary>
    /// Derives the numeric code a client can branch on. It is computed from the key rather than assigned, so adding a rule cannot renumber
    /// the existing ones.
    /// </summary>
    ///
    /// <param name="key">The message key the failure reports, resolved into a sentence only when the response is written.</param>
    ///
    /// <returns>The numeric error code.</returns>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    private static long ToNumericCode(string key)
    {
        byte[] hash = SHA256.HashData(Encoding.UTF8.GetBytes(key));

        return BinaryPrimitives.ReadUInt32BigEndian(hash);
    }

    /// <summary>
    /// Derives the machine-readable name from the key, which is what a client matches on rather than the human sentence beside it.
    /// </summary>
    ///
    /// <param name="key">The message key the failure reports, resolved into a sentence only when the response is written.</param>
    ///
    /// <returns>The public error name.</returns>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    private static string ToErrorName(string key)
    {
        if (string.IsNullOrEmpty(key))
            return "validation_error";

        StringBuilder builder = new(key.Length);

        foreach (char character in key)
        {
            if (char.IsLetterOrDigit(character))
            {
                builder.Append(char.ToLowerInvariant(character));

                continue;
            }

            if (builder.Length > 0 && builder[^1] != '_')
                builder.Append('_');
        }

        if (builder.Length > 0 && builder[^1] == '_')
            builder.Length--;

        return builder.Length == 0 ? "validation_error" : builder.ToString();
    }
}
