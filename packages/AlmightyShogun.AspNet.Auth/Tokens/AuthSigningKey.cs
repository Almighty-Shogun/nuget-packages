using System.Text;
using Microsoft.IdentityModel.Tokens;
using System.ComponentModel.DataAnnotations;

namespace AlmightyShogun.AspNet.Auth;

/// <summary>
/// Turns the configured secret into the symmetric key HMAC-SHA256 signs and validates with. Both sides of the package go
/// through here, so a secret this package considers too short is refused the same way whether a token is being minted or
/// checked.
/// </summary>
///
/// <author>Almighty-Shogun</author>
/// <since>Unreleased</since>
internal static class AuthSigningKey
{
    /// <summary>
    /// The shortest signing key this package accepts, in bytes. It is a policy of its own, matching the digest size
    /// RFC 7518 recommends for HS256, and is stricter than the library's own floor of 128 bits, so a secret between the
    /// two would sign and validate but is refused here.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    internal const int MinimumSecretBytes = 32;

    /// <summary>
    /// Builds the signing key, checking the secret in bytes rather than characters, which is what a key length actually
    /// measures and what <see cref="MinLengthAttribute"/> on a string cannot express.
    /// </summary>
    ///
    /// <param name="secret">The configured symmetric secret, encoded as UTF-8 to obtain the key bytes.</param>
    ///
    /// <returns>The symmetric key both token signing and token validation use.</returns>
    ///
    /// <exception cref="InvalidOperationException">
    /// The secret encodes to fewer than <see cref="MinimumSecretBytes"/> bytes. Not
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
