namespace AlmightyShogun.AspNet.Auth.Credentials;

/// <summary>
/// Holds the fixed timings the session service runs on.
/// </summary>
///
/// <remarks>
/// Non-generic on purpose. The session service is generic over the user type, and a static field inside a generic type
/// is allocated once per constructed type, which gives a value that is the same for every user type more copies than it
/// needs.
/// </remarks>
///
/// <author>Almighty-Shogun</author>
/// <since>4.0.0</since>
internal static class AuthSessionDefaults
{
    /// <summary>
    /// How long after a rotation a spent refresh token is refused without revoking anything, covering the window where a
    /// client retried before it stored the new token. The spent token is never accepted either way, since it no longer
    /// matches; outside this window the first presentation is treated as a replay and ends every session the user holds.
    /// The session's record of that token goes with the detection, so presenting it again afterwards revokes nothing.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>4.0.0</since>
    internal static readonly TimeSpan RotationGrace = TimeSpan.FromSeconds(30);

    /// <summary>
    /// How many times the write of a detected replay's revocations is attempted before it is given up on. A rotation of
    /// one of the user's other sessions committing mid-detection leaves the revocation of that row matching nothing, and
    /// it is reapplied over what the rotation wrote; this bounds that so a steady stream of refreshes cannot hold the
    /// write open indefinitely.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>4.1.0</since>
    internal const int RevocationSaveAttempts = 5;

    /// <summary>
    /// How many times an operation that changes a credential or redeems a token is run before it is given up on. Lower
    /// than <see cref="RevocationSaveAttempts"/> because an attempt on the password paths verifies the stored hash again,
    /// which is deliberately slow, so it costs far more than a revocation's attempt does.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    internal const int CredentialWriteAttempts = 3;
}
