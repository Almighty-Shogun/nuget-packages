using System.ComponentModel.DataAnnotations;

namespace AlmightyShogun.AspNet.Auth.Credentials;

/// <summary>
/// Configures how repeated failed logins lock an account. Every value here is read only while <see cref="Enabled"/> is
/// set, which it is not by default.
/// </summary>
///
/// <author>Almighty-Shogun</author>
/// <since>4.0.0</since>
public sealed record LockoutPolicy
{
    /// <summary>
    /// Whether failed attempts lock the account. Off by default, because locking on failure count alone lets someone
    /// lock a user out simply by guessing at their username.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>4.0.0</since>
    public bool Enabled { get; init; } = false;

    /// <summary>
    /// How many consecutive failures lock the account. The count resets on a completed sign-in, so it measures a run of
    /// failures rather than a lifetime total. A user owing a second factor completes on the code, not on the password,
    /// so a correct password that only buys a challenge leaves the run exactly as it found it.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>4.0.0</since>
    [Range(1, int.MaxValue)]
    public int MaxFailedAttempts { get; init; } = 5;

    /// <summary>
    /// How long the lockout lasts, in minutes. It expires on its own, so a locked-out user needs no administrator to
    /// get back in.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>4.0.0</since>
    [Range(1, int.MaxValue)]
    public int DurationMinutes { get; init; } = 15;
}
