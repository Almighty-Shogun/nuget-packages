using System.Text;
using System.Security.Cryptography;

namespace AlmightyShogun.AspNet.CredentialAuth;

/// <summary>
/// Hashes the tokens this package stores, so a database copy yields nothing usable. Deliberately a plain digest rather
/// than a password hash: these values are long random strings, not guessable secrets, so the work factor a password
/// needs would only slow every lookup down. Application code needs it to match a token it holds against a stored row.
/// </summary>
///
/// <author>Almighty-Shogun</author>
/// <since>Unreleased</since>
public static class TokenHasher
{
    /// <summary>
    /// Hashes a token for storage or lookup. The same input always produces the same output, which is what lets a
    /// presented token be found by its hash instead of compared row by row.
    /// </summary>
    ///
    /// <param name="token">The token exactly as it was issued to or presented by the client.</param>
    ///
    /// <returns>The digest as uppercase hexadecimal, stable enough to store in a column and index on.</returns>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    public static string Hash(string token)
    {
        byte[] bytes = SHA256.HashData(Encoding.UTF8.GetBytes(token));

        return Convert.ToHexString(bytes);
    }
}
