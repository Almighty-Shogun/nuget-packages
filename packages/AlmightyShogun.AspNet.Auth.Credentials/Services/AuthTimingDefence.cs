using Microsoft.AspNetCore.Identity;

namespace AlmightyShogun.AspNet.Auth.Credentials;

/// <summary>
/// Provides decoy password verification to reduce timing differences for unknown users.
/// </summary>
///
/// <remarks>
/// Only password verification time is equalized; known users may incur additional
/// lockout operations. Non-generic to share the decoy hash across user types.
/// </remarks>
///
/// <author>Almighty-Shogun</author>
/// <since>4.0.0</since>
internal static class AuthTimingDefence
{
    /// <summary>
    /// The temporary user used for decoy password verification.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>4.0.0</since>
    private static readonly AuthUser _decoyUser = new() { Username = "decoy", Email = "decoy" };

    /// <summary>
    /// The password hasher used for decoy verification.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>4.0.0</since>
    private static readonly PasswordHasher<AuthUser> _decoyHasher = new();

    /// <summary>
    /// The lazily initialized decoy hash used to simulate password verification.
    /// </summary>
    ///
    /// <remarks>
    /// The first unknown-user sign-in also incurs the hash creation cost.
    /// </remarks>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>4.0.0</since>
    private static readonly string _decoyHash = _decoyHasher.HashPassword(_decoyUser, "decoy");

    /// <summary>
    /// Performs a decoy password verification for an unknown user.
    /// </summary>
    ///
    /// <param name="password">The submitted password.</param>
    ///
    /// <returns>The verification result, which callers ignore.</returns>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>4.0.0</since>
    internal static PasswordVerificationResult SpendVerification(string password)
        => _decoyHasher.VerifyHashedPassword(_decoyUser, _decoyHash, password);
}
