using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AlmightyShogun.AspNet.Auth.Credentials;

/// <summary>
/// Represents a single-use two-factor recovery code.
/// </summary>
///
/// <author>Almighty-Shogun</author>
/// <since>4.0.0</since>
[Table("two_factor_recovery_codes")]
public sealed class TwoFactorRecoveryCode
{
    /// <summary>
    /// The recovery code's primary key.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>4.0.0</since>
    public int Id { get; set; }

    /// <summary>
    /// The identifier of the associated two-factor enrollment.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>4.0.0</since>
    public int UserTwoFactorId { get; set; }

    /// <summary>
    /// The hash of the recovery code.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>4.0.0</since>
    [Required]
    [MaxLength(64)]
    public string CodeHash { get; set; } = string.Empty;

    /// <summary>
    /// The timestamp when the recovery code was used, or null if unused.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>4.0.0</since>
    public DateTimeOffset? UsedAt { get; set; }
}
