using System.Text;
using System.Buffers.Binary;
using System.Security.Cryptography;

namespace AlmightyShogun.AspNet.Validation;

/// <summary>
/// Represents a single internal validation error.
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
    /// Creates a validation error from a message key and message parameters.
    /// </summary>
    ///
    /// <param name="key">The validation message key.</param>
    /// <param name="parameters">The validation message parameters.</param>
    ///
    /// <returns>The validation error.</returns>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    public static ValidationError From(string key, object?[] parameters)
        => new(ToNumericCode(key), key, ToErrorName(key), parameters);

    /// <summary>
    /// Converts a validation message key into a stable numeric error code.
    /// </summary>
    ///
    /// <param name="key">The validation message key.</param>
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
    /// Converts a validation message key into a public error name.
    /// </summary>
    ///
    /// <param name="key">The validation message key.</param>
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
