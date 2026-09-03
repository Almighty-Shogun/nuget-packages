using Microsoft.AspNetCore.Identity;

namespace AlmightyShogun.AspNet.Auth.Credentials;

/// <summary>
/// Holds the fixed values a sign-in path uses to spend the same work on an unknown identifier as on a real one, so the
/// response time does not tell an attacker which usernames exist.
/// </summary>
///
/// <remarks>
/// Non-generic on purpose. The services that use these are generic over the user type, and a static field inside a
/// generic type is allocated once per constructed type, so the hashing below would repeat for every <c>TUser</c> an
/// application happens to close over.
/// </remarks>
///
/// <author>Almighty-Shogun</author>
/// <since>Unreleased</since>
internal static class AuthTimingDefence
{
    /// <summary>
    /// The stand-in user the decoy hash was produced for. Never persisted, and never returned to a caller.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    private static readonly AuthUser _decoyUser = new() { Username = "decoy", Email = "decoy" };

    /// <summary>
    /// The hasher used for the decoy verification, matching the one a real sign-in uses so the work is comparable.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    private static readonly PasswordHasher<AuthUser> _decoyHasher = new();

    /// <summary>
    /// A hash computed once at startup, so verifying against it costs what verifying a real password costs.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    private static readonly string _decoyHash = _decoyHasher.HashPassword(_decoyUser, "decoy");

    /// <summary>
    /// Spends a password verification against the decoy hash, for the path where no user matched the identifier.
    /// </summary>
    ///
    /// <param name="password">The submitted password, verified and then discarded.</param>
    ///
    /// <returns>The verification outcome, which the caller ignores; only the time it took matters.</returns>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    internal static PasswordVerificationResult SpendVerification(string password)
        => _decoyHasher.VerifyHashedPassword(_decoyUser, _decoyHash, password);
}
