using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AlmightyShogun.AspNet.Auth.Credentials;

/// <summary>
/// A user's two-factor enrolment. Kept out of the user row because the secret is only needed while verifying a code,
/// and every ordinary read of a user would otherwise load it.
/// </summary>
///
/// <author>Almighty-Shogun</author>
/// <since>Unreleased</since>
[Table("user_two_factors")]
public sealed class UserTwoFactor
{
    /// <summary>
    /// The surrogate key. The enrolment is always reached through its user, so this value appears in no
    /// response and in no query a caller writes.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    public int Id { get; set; }

    /// <summary>
    /// The user this enrolment belongs to. Unique, so a user has at most one enrolment.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    public int UserId { get; set; }

    /// <summary>
    /// Whether a confirmed enrolment is in force. Unset while an enrolment has been begun but not confirmed,
    /// which is a state the row exists in too, so its presence is not the answer to whether two-factor is on.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    public bool IsEnabled { get; set; }

    /// <summary>
    /// The protected TOTP shared secret currently in force. Stored encrypted, so a database copy alone does not let an
    /// attacker mint valid codes. The column is sized for the protected form, which is several times the secret itself.
    /// Empty until an enrolment has been confirmed, and only ever replaced by a confirmed <see cref="PendingSecret"/>.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    [Required]
    [MaxLength(512)]
    public string Secret { get; set; } = string.Empty;

    /// <summary>
    /// The protected secret an enrolment in progress is offering, or <c>null</c> when none is outstanding.
    /// Held apart from <see cref="Secret"/> so that a secret on offer is never one in force. See
    /// <see cref="IAuthTwoFactorService{TUser}"/> for when it moves across.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    [MaxLength(512)]
    public string? PendingSecret { get; set; }

    /// <summary>
    /// When the outstanding <see cref="PendingSecret"/> stops being confirmable, or <c>null</c> when none is
    /// outstanding. Set <see cref="TwoFactorPolicy.PendingSecretMinutes"/> ahead when an enrolment begins, and cleared
    /// again when one is confirmed.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    public DateTimeOffset? PendingSecretExpiresAt { get; set; }

    /// <summary>
    /// The last TOTP time step accepted for this user, or <c>null</c> before any code has been accepted.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    public long? LastWindow { get; set; }

    /// <summary>
    /// When the enrolment row was created, which is when an enrolment was first begun rather than when one
    /// was confirmed. Beginning enrolment again reuses the row, so this is not moved forward, but disabling deletes it
    /// and a later enrolment then starts a new row with a fresh value.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;

    /// <summary>
    /// The recovery codes issued when the enrolment was confirmed, spent ones included, since a spent code
    /// keeps its row. Verification filters them by <see cref="TwoFactorRecoveryCode.UsedAt"/> rather than relying on this.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    public List<TwoFactorRecoveryCode> RecoveryCodes { get; set; } = [];
}
