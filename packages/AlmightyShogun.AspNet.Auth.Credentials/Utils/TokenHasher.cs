using System.Text;
using System.Security.Cryptography;

namespace AlmightyShogun.AspNet.Auth.Credentials;

/// <summary>
/// Provides SHA-256 hashing for authentication tokens.
/// </summary>
///
/// <author>Almighty-Shogun</author>
/// <since>4.0.0</since>
public static class TokenHasher
{
    /// <summary>
    /// Computes the SHA-256 hash of a token.
    /// </summary>
    ///
    /// <param name="token">The token to hash.</param>
    ///
    /// <returns>The hash as an uppercase hexadecimal string.</returns>
    ///
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="token"/> is null.</exception>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>4.0.0</since>
    public static string Hash(string token)
    {
        byte[] bytes = SHA256.HashData(Encoding.UTF8.GetBytes(token));

        return Convert.ToHexString(bytes);
    }
}
