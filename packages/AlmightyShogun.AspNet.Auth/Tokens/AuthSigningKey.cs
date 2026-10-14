using System.Text;
using Microsoft.IdentityModel.Tokens;
using System.ComponentModel.DataAnnotations;

namespace AlmightyShogun.AspNet.Auth;

/// <summary>
/// Creates symmetric signing keys from configured secrets.
/// </summary>
/// <author>Almighty-Shogun</author>
/// <since>4.0.0</since>
internal static class AuthSigningKey
{
    /// <summary>
    /// The minimum signing key length in bytes.
    /// </summary>
    /// <author>Almighty-Shogun</author>
    /// <since>4.0.0</since>
    internal const int MinimumSecretBytes = 32;

    /// <summary>
    /// Creates a symmetric signing key from a UTF-8 encoded secret.
    /// </summary>
    /// <param name="secret">The signing secret.</param>
    /// <returns>The symmetric security key.</returns>
    /// <exception cref="InvalidOperationException">
    /// Thrown when the encoded secret is shorter than the required minimum.
    /// </exception>
    /// <author>Almighty-Shogun</author>
    /// <since>4.0.0</since>
    internal static SymmetricSecurityKey Create(string secret)
    {
        byte[] key = Encoding.UTF8.GetBytes(secret);

        return key.Length >= MinimumSecretBytes
            ? new SymmetricSecurityKey(key)
            : throw new InvalidOperationException(
                $"Auth:Secret must encode to at least {MinimumSecretBytes} bytes for HMAC-SHA256 signing, but is {key.Length}."
            );
    }
}
