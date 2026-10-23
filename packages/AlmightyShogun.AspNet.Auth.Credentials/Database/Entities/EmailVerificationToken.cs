using System.Diagnostics.CodeAnalysis;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AlmightyShogun.AspNet.Auth.Credentials;

/// <summary>
/// Represents an issued email verification token, including redeemed tokens.
/// </summary>
///
/// <author>Almighty-Shogun</author>
/// <since>4.0.0</since>
[Table("email_verification_tokens")]
[SuppressMessage("ReSharper", "PropertyCanBeMadeInitOnly.Global")]
public sealed class EmailVerificationToken
{
    /// <summary>
    /// The token's primary key.
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
    /// The hash of the issued verification token.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>4.0.0</since>
    [Required]
    [MaxLength(64)]
    public string TokenHash { get; set; } = string.Empty;

    /// <summary>
    /// The email address being verified.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>4.0.0</since>
    [Required]
    [MaxLength(255)]
    public string Email { get; set; } = string.Empty;

    /// <summary>
    /// The verification flow for which the token was issued.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>4.1.0</since>
    public EmailVerificationPurpose Purpose { get; set; }

    /// <summary>
    /// The token creation timestamp.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>4.0.0</since>
    public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;

    /// <summary>
    /// The token expiration timestamp.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>4.0.0</since>
    public DateTimeOffset ExpiresAt { get; set; }

    /// <summary>
    /// The token redemption or invalidation timestamp, if any.
    /// </summary>
    ///
    /// <remarks>
    /// Database updates may bypass change tracking, leaving loaded instances stale.
    /// </remarks>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>4.0.0</since>
    public DateTimeOffset? UsedAt { get; set; }

    /// <summary>
    /// Indicates whether the token is unused and unexpired.
    /// </summary>
    ///
    /// <remarks>
    /// Evaluates the current instance only. Redemption also validates the token's
    /// purpose and other conditions directly in the database.
    /// </remarks>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>4.0.0</since>
    [NotMapped]
    public bool IsActive => UsedAt is null && ExpiresAt > DateTimeOffset.UtcNow;
}
