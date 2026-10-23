using System.Diagnostics.CodeAnalysis;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AlmightyShogun.AspNet.Auth.Credentials;

/// <summary>
/// Represents a user's password reset token, replaced when a new reset is requested.
/// </summary>
///
/// <author>Almighty-Shogun</author>
/// <since>4.0.0</since>
[Table("password_reset_tokens")]
[SuppressMessage("ReSharper", "PropertyCanBeMadeInitOnly.Global")]
public sealed class PasswordResetToken
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
    /// The hash of the issued password reset token.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>4.0.0</since>
    [Required]
    [MaxLength(64)]
    public string TokenHash { get; set; } = string.Empty;

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
    /// The token redemption timestamp, or null if unused.
    /// </summary>
    /// 
    /// <remarks>
    /// Reset when a new password reset token is issued.
    /// </remarks>
    /// 
    /// <author>Almighty-Shogun</author>
    /// <since>4.0.0</since>
    public DateTimeOffset? UsedAt { get; set; }

    /// <summary>
    /// The IP address from which the password reset was requested.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>4.0.0</since>
    [MaxLength(45)]
    public string? RequestedIpAddress { get; set; }

    /// <summary>
    /// Indicates whether the token is unused and unexpired.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>4.0.0</since>
    [NotMapped]
    public bool IsActive => UsedAt is null && ExpiresAt > DateTimeOffset.UtcNow;
}
