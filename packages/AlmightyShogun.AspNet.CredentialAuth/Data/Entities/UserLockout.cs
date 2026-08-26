using System.ComponentModel.DataAnnotations.Schema;

namespace AlmightyShogun.AspNet.CredentialAuth;

/// <summary>
/// A user's failed sign-in run and the lockout it earned. Kept out of the user row because lockout is off by default,
/// so a deployment that never enables it never writes here and the user table stays free of columns it does not use.
/// </summary>
///
/// <remarks>
/// A row exists only between the first failure and the next success. Signing in deletes it rather than zeroing it, so
/// the table holds one row per account currently failing rather than one per account that ever has.
/// </remarks>
///
/// <author>Almighty-Shogun</author>
/// <since>Unreleased</since>
[Table("user_lockouts")]
public sealed class UserLockout
{
    /// <summary>
    /// Gets or sets the surrogate key. The row is always reached through its user, so this value appears in no response
    /// and in no query a caller writes.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    public int Id { get; set; }

    /// <summary>
    /// Gets or sets the user this run of failures belongs to. Unique, so an account cannot accumulate two counters.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    public int UserId { get; set; }

    /// <summary>
    /// Gets or sets how many failures have happened in a row. Reset to zero when the count reaches the configured limit
    /// and the lockout is applied, so it counts towards the next lockout rather than a lifetime total.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    public int AccessFailedCount { get; set; }

    /// <summary>
    /// Gets or sets when the current lockout expires, or <c>null</c> while failures are only being counted. Stored
    /// rather than derived, so a lockout survives a restart.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    public DateTimeOffset? LockoutEnd { get; set; }

    /// <summary>
    /// Gets whether the account is locked right now, which is a lockout end in the future rather than merely one that
    /// was set. An expired lockout needs no clearing before the next attempt.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    [NotMapped]
    public bool IsLocked => LockoutEnd is not null && LockoutEnd > DateTimeOffset.UtcNow;
}
