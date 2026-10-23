using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AlmightyShogun.AspNet.Auth.Credentials;

/// <summary>
/// Represents a user's two-factor authentication enrollment.
/// </summary>
///
/// <author>Almighty-Shogun</author>
/// <since>4.0.0</since>
[Table("user_two_factors")]
public sealed class UserTwoFactor
{
    /// <summary>
    /// The enrollment's primary key.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>4.0.0</since>
    public int Id { get; set; }

    /// <summary>
    /// The identifier of the associated user.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>4.0.0</since>
    public int UserId { get; set; }

    /// <summary>
    /// Indicates whether two-factor authentication has been confirmed and enabled.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>4.0.0</since>
    public bool IsEnabled { get; set; }

    /// <summary>
    /// The protected TOTP secret used for authentication.
    /// </summary>
    ///
    /// <remarks>
    /// Stored encrypted and activated only after enrollment confirmation.
    /// </remarks>
    /// 
    /// <author>Almighty-Shogun</author>
    /// <since>4.0.0</since>
    [Required]
    [MaxLength(512)]
    public string Secret { get; set; } = string.Empty;

    /// <summary>
    /// The protected TOTP secret awaiting enrollment confirmation, if any.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>4.0.0</since>
    [MaxLength(512)]
    public string? PendingSecret { get; set; }

    /// <summary>
    /// The pending secret's expiration timestamp, or null if none is pending.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>4.0.0</since>
    public DateTimeOffset? PendingSecretExpiresAt { get; set; }

    /// <summary>
    /// The last accepted TOTP time step, used to prevent code reuse.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>4.0.0</since>
    public long? LastWindow { get; set; }

    /// <summary>
    /// The enrollment creation timestamp.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>4.0.0</since>
    public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;

    /// <summary>
    /// The recovery codes associated with this enrollment, including used codes.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>4.0.0</since>
    public List<TwoFactorRecoveryCode> RecoveryCodes { get; set; } = [];
}
