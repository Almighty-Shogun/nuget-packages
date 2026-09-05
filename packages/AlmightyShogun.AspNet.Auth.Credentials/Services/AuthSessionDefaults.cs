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
/// <since>Unreleased</since>
internal static class AuthSessionDefaults
{
    /// <summary>
    /// How long after a rotation a spent refresh token is refused without revoking anything, covering the window where a
    /// client retried before it stored the new token. The spent token is never accepted either way, since it no longer
    /// matches; outside this window presenting it is treated as a replay and ends every session the user holds.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    internal static readonly TimeSpan RotationGrace = TimeSpan.FromSeconds(30);
}
