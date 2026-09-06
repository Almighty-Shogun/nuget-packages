using System.ComponentModel.DataAnnotations.Schema;

namespace AlmightyShogun.AspNet.Auth.Credentials;

/// <summary>
/// A user's failed sign-in run and the lockout it earned. Kept out of the user row because lockout is off by default,
/// so a deployment that never enables it never writes here and the user table stays free of columns it does not use.
/// </summary>
///
/// <remarks>
/// A row exists only between the first failure and the sign-in that next completes. Completing one deletes the row
/// rather than zeroing it, so the table holds one row per account currently failing rather than one per account that
/// ever has. Where a second factor is owed the row outlives the password and is deleted only once the code is
/// presented, since a correct password on its own finishes nothing.
/// </remarks>
///
/// <author>Almighty-Shogun</author>
/// <since>4.0.0</since>
[Table("user_lockouts")]
public sealed class UserLockout
{
    /// <summary>
    /// The surrogate key. The row is always reached through its user, so this value appears in no response
    /// and in no query a caller writes.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>4.0.0</since>
    public int Id { get; set; }

    /// <summary>
    /// The user this run of failures belongs to. Unique, so an account cannot accumulate two counters.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>4.0.0</since>
    public int UserId { get; set; }

    /// <summary>
    /// How many failures stand against the account, which never exceeds the configured limit.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>4.0.0</since>
    public int AccessFailedCount { get; set; }

    /// <summary>
    /// When the current lockout expires, or <c>null</c> while failures are only being counted. Stored
    /// rather than derived, so a lockout survives a restart.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>4.0.0</since>
    public DateTimeOffset? LockoutEnd { get; set; }

    /// <summary>
    /// Whether the account is locked right now, which is a lockout end in the future rather than merely one that
    /// was set. An expired lockout needs no clearing before the next attempt.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>4.0.0</since>
    [NotMapped]
    public bool IsLocked => LockoutEnd is not null && LockoutEnd > DateTimeOffset.UtcNow;
}
