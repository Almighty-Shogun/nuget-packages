using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AlmightyShogun.AspNet.CredentialAuth;

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
    /// Gets or sets the surrogate key. The enrolment is always reached through its user, so this value appears in no
    /// response and in no query a caller writes.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    public int Id { get; set; }

    /// <summary>
    /// Gets or sets the user this enrolment belongs to. Unique, so a user has at most one enrolment.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    public int UserId { get; set; }

    /// <summary>
    /// Gets or sets whether a second factor is required to sign in. Set only once an enrolment has been confirmed with
    /// a valid code, so an abandoned enrolment cannot lock the owner out.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    public bool IsEnabled { get; set; }

    /// <summary>
    /// Gets or sets the protected TOTP shared secret. Stored encrypted, so a database copy alone does not let an
    /// attacker mint valid codes. The column is sized for the protected form, which is several times the secret itself.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    [Required]
    [MaxLength(512)]
    public string Secret { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the last TOTP time step accepted for this user. A code from that step or earlier is refused, which
    /// is what stops one intercepted code being used twice inside its validity window.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    public long? LastWindow { get; set; }

    /// <summary>
    /// Gets or sets when the enrolment was created, for showing a user when their second factor was set up.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;

    /// <summary>
    /// Gets or sets the unspent recovery codes issued when the enrolment was confirmed.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    public List<TwoFactorRecoveryCode> RecoveryCodes { get; set; } = [];
}
