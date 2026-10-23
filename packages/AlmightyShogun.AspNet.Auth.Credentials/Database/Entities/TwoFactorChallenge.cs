using System.Diagnostics.CodeAnalysis;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AlmightyShogun.AspNet.Auth.Credentials;

/// <summary>
/// Represents a pending two-factor login challenge, including redeemed challenges.
/// </summary>
///
/// <author>Almighty-Shogun</author>
/// <since>4.1.0</since>
[Table("two_factor_challenges")]
[SuppressMessage("ReSharper", "PropertyCanBeMadeInitOnly.Global")]
public sealed class TwoFactorChallenge
{
    /// <summary>
    /// The challenge's primary key.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>4.1.0</since>
    public int Id { get; set; }

    /// <summary>
    /// The identifier of the associated user.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>4.1.0</since>
    public int UserId { get; set; }

    /// <summary>
    /// The hash of the issued challenge token.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>4.1.0</since>
    [Required]
    [MaxLength(64)]
    public string TokenHash { get; set; } = string.Empty;

    /// <summary>
    /// The application associated with the challenge, if scoped.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>4.1.0</since>
    [MaxLength(255)]
    public string? App { get; set; }

    /// <summary>
    /// The challenge creation timestamp.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>4.1.0</since>
    public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;

    /// <summary>
    /// The challenge expiration timestamp.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>4.1.0</since>
    public DateTimeOffset ExpiresAt { get; set; }

    /// <summary>
    /// The challenge redemption or invalidation timestamp, if any.
    /// </summary>
    ///
    /// <remarks>
    /// Completing a login, issuing another challenge, or changing the password
    /// invalidates the challenge. Some updates bypass change tracking.
    /// </remarks>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>4.1.0</since>
    public DateTimeOffset? UsedAt { get; set; }

    /// <summary>
    /// The IP address from which the login was requested.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>4.1.0</since>
    [MaxLength(45)]
    public string? RequestedIpAddress { get; set; }

    /// <summary>
    /// Indicates whether the challenge is unused and unexpired.
    /// </summary>
    ///
    /// <remarks>
    /// Evaluates the current instance only. Completion validates and claims
    /// the challenge directly in the database.
    /// </remarks>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>4.1.0</since>
    [NotMapped]
    public bool IsActive => UsedAt is null && ExpiresAt > DateTimeOffset.UtcNow;
}
