using System.Text;
using Microsoft.IdentityModel.Tokens;
using System.ComponentModel.DataAnnotations;

namespace AlmightyShogun.AspNet.JwtAuth;

/// <summary>
/// Turns the configured secret into the symmetric key HMAC-SHA256 signs and validates with. Both sides of the package go
/// through here, so a secret the algorithm would refuse is refused the same way whether a token is being minted or
/// checked.
/// </summary>
///
/// <author>Almighty-Shogun</author>
/// <since>Unreleased</since>
internal static class AuthSigningKey
{
    /// <summary>
    /// The shortest signing key HMAC-SHA256 accepts, in bytes. Anything shorter is refused by the algorithm itself, so
    /// it is rejected here rather than surfacing as a signing failure on the first request.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    internal const int MinimumSecretBytes = 32;

    /// <summary>
    /// Builds the signing key, checking the secret in bytes rather than characters, which is what the algorithm actually
    /// constrains and what <see cref="MinLengthAttribute"/> on a string cannot express.
    /// </summary>
    ///
    /// <param name="secret">The configured symmetric secret, encoded as UTF-8 to obtain the key bytes.</param>
    ///
    /// <returns>The symmetric key both token signing and token validation use.</returns>
    ///
    /// <exception cref="InvalidOperationException">
    /// The secret encodes to fewer than <see cref="MinimumSecretBytes"/> bytes, so HMAC-SHA256 would refuse it. Not
    /// reachable while startup validation is on, since <see cref="MinLengthAttribute"/> on <see cref="AuthSettings.Secret"/>
    /// already demands that many characters and a character never encodes to less than a byte.
    /// </exception>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
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
