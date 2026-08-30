using System.Text.Json.Serialization;
using System.Diagnostics.CodeAnalysis;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AlmightyShogun.AspNet.CredentialAuth;

/// <summary>
/// One issued password reset. A spent row is kept rather than deleted, so the resets issued against an account stay
/// auditable; redeeming a spent token is refused exactly as an unknown one is.
/// </summary>
///
/// <author>Almighty-Shogun</author>
/// <since>Unreleased</since>
[Table("password_reset_tokens")]
[SuppressMessage("ReSharper", "PropertyCanBeMadeInitOnly.Global")]
public sealed class PasswordResetToken
{
    /// <summary>
    /// Gets or sets the surrogate key. It is never handed to a client: the emailed token is the only handle a caller
    /// has on this row, so the key can stay a plain incrementing integer.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    public int Id { get; set; }

    /// <summary>
    /// Gets or sets the user the reset was issued for. Cascades with the user.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    public int UserId { get; set; }

    /// <summary>
    /// Gets or sets the hash of the token that was emailed. Only the hash is stored, so the value in the email cannot be
    /// recovered from the database.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    [Required]
    [JsonIgnore]
    [MaxLength(64)]
    public string TokenHash { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets when the reset was requested. Kept after the token is spent, so a burst of requests against one
    /// account is still visible afterwards.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;

    /// <summary>
    /// Gets or sets when the token stops being usable, set from the configured reset lifetime at issue.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    public DateTimeOffset ExpiresAt { get; set; }

    /// <summary>
    /// Gets or sets when the token was spent, or <c>null</c> while it is still usable. Set instead of deleting the row, so
    /// the redemption stays visible afterwards; a second attempt with the same value is refused as if unknown.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    public DateTimeOffset? UsedAt { get; set; }

    /// <summary>
    /// Gets or sets the address the reset was requested from, kept for auditing an unexpected reset.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    [MaxLength(45)]
    public string? RequestedIpAddress { get; set; }

    /// <summary>
    /// Gets whether the token would still be accepted, which is unspent and not past its expiry.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    [NotMapped]
    public bool IsActive => UsedAt is null && ExpiresAt > DateTimeOffset.UtcNow;
}
