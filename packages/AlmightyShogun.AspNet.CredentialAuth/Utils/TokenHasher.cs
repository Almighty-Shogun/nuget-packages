using System.Text;
using System.Security.Cryptography;

namespace AlmightyShogun.AspNet.CredentialAuth;

/// <summary>
/// Provides hashing helpers for tokens that must not be stored as plain text.
/// </summary>
///
/// <author>Almighty-Shogun</author>
/// <since>Unreleased</since>
internal static class TokenHasher
{
    /// <summary>
    /// Hashes a token value.
    /// </summary>
    ///
    /// <param name="token">The token to hash.</param>
    ///
    /// <returns>The hexadecimal token hash.</returns>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    public static string Hash(string token)
    {
        byte[] bytes = SHA256.HashData(Encoding.UTF8.GetBytes(token));
        return Convert.ToHexString(bytes);
    }
}
