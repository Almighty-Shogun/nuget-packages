using System.ComponentModel.DataAnnotations;

namespace AlmightyShogun.AspNet.Auth.Credentials;

/// <summary>
/// Configures how repeated failed logins lock an account. Disabled by default, because locking on failure count alone
/// lets anyone deny service to a known account by failing logins against it deliberately.
/// </summary>
///
/// <author>Almighty-Shogun</author>
/// <since>Unreleased</since>
public sealed record LockoutPolicy
{
    /// <summary>
    /// Gets whether failed attempts lock the account. Off by default, because locking on failure count alone lets someone
    /// lock a user out simply by guessing at their username.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    public bool Enabled { get; init; } = false;

    /// <summary>
    /// Gets how many consecutive failures lock the account. The count resets on any successful sign-in, so it measures a
    /// run of failures rather than a lifetime total.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    [Range(1, int.MaxValue)]
    public int MaxFailedAttempts { get; init; } = 5;

    /// <summary>
    /// Gets how long the lockout lasts, in minutes. It expires on its own, so a locked-out user needs no administrator to
    /// get back in.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    [Range(1, int.MaxValue)]
    public int DurationMinutes { get; init; } = 15;
}
