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
    /// How long a just-rotated refresh token is still accepted, covering the window where a client retried before it
    /// stored the new token. Outside it, presenting a spent token is treated as a replay rather than a race.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    internal static readonly TimeSpan RotationGrace = TimeSpan.FromSeconds(30);
}
