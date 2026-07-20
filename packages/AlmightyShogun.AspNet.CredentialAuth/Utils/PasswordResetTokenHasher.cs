using System.Text;
using System.Security.Cryptography;

namespace AlmightyShogun.AspNet.CredentialAuth;

/// <summary>
/// Provides hashing helpers for password reset tokens.
/// </summary>
///
/// <author>Almighty-Shogun</author>
/// <since>Unreleased</since>
internal static class PasswordResetTokenHasher
{
    /// <summary>
    /// Hashes a password reset token.
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
