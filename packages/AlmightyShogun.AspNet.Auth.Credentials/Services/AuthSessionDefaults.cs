namespace AlmightyShogun.AspNet.Auth.Credentials;

/// <summary>
/// Defines shared timing and retry limits for authentication services.
/// </summary>
///
/// <remarks>
/// Non-generic to avoid duplicating static fields for each user type.
/// </remarks>
///
/// <author>Almighty-Shogun</author>
/// <since>4.0.0</since>
internal static class AuthSessionDefaults
{
    /// <summary>
    /// Grace period before a reused refresh token triggers session revocation.
    /// Reuse within this period is rejected without revoking sessions.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>4.0.0</since>
    internal static readonly TimeSpan RotationGrace = TimeSpan.FromSeconds(30);

    /// <summary>
    /// Maximum attempts to persist session revocations after token reuse.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>4.1.0</since>
    internal const int RevocationSaveAttempts = 5;

    /// <summary>
    /// Maximum retries for credential updates and token redemption.
    /// Lower than <see cref="RevocationSaveAttempts"/> due to password hashing costs.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    internal const int CredentialWriteAttempts = 3;
}
