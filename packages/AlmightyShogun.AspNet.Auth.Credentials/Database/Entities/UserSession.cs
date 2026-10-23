using System.Diagnostics.CodeAnalysis;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AlmightyShogun.AspNet.Auth.Credentials;

/// <summary>
/// Represents an authenticated user session managed through refresh tokens.
/// </summary>
///
/// <author>Almighty-Shogun</author>
/// <since>4.0.0</since>
[Table("user_sessions")]
[SuppressMessage("ReSharper", "PropertyCanBeMadeInitOnly.Global")]
public sealed class UserSession
{
    /// <summary>
    /// The session's primary key.
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
    /// The hash of the current refresh token.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>4.0.0</since>
    [Required]
    [MaxLength(64)]
    public string RefreshTokenHash { get; set; } = string.Empty;

    /// <summary>
    /// The application associated with the session, if scoped.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>4.0.0</since>
    [MaxLength(255)]
    public string? App { get; set; }

    /// <summary>
    /// The session expiration timestamp, extended on refresh up to its absolute lifetime.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>4.0.0</since>
    public DateTimeOffset ExpiresAt { get; set; }

    /// <summary>
    /// The session creation timestamp, used to enforce its absolute lifetime.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>4.0.0</since>
    public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;

    /// <summary>
    /// The timestamp of the session's last refresh.
    /// </summary>
    ///
    /// <remarks>
    /// Also used to determine the refresh token rotation grace period.
    /// </remarks>
    /// 
    /// <author>Almighty-Shogun</author>
    /// <since>4.0.0</since>
    public DateTimeOffset LastActiveAt { get; set; } = DateTimeOffset.UtcNow;

    /// <summary>
    /// Indicates whether the session has been revoked.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>4.0.0</since>
    public bool IsRevoked { get; set; }

    /// <summary>
    /// The IP address from which the session was last used.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>4.0.0</since>
    [MaxLength(45)]
    public string? IpAddress { get; set; }

    /// <summary>
    /// The raw user agent associated with the session.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>4.0.0</since>
    [MaxLength(512)]
    public string? UserAgent { get; set; }

    /// <summary>
    /// The device identified from the user agent, if available.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>4.0.0</since>
    [MaxLength(256)]
    public string? Device { get; set; }

    /// <summary>
    /// The browser identified from the user agent, if available.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>4.0.0</since>
    [MaxLength(256)]
    public string? Browser { get; set; }

    /// <summary>
    /// The operating system identified from the user agent, if available.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>4.0.0</since>
    [MaxLength(256)]
    public string? Os { get; set; }

    /// <summary>
    /// The hash of the previously rotated refresh token, used for replay detection.
    /// </summary>
    ///
    /// <remarks>
    /// Cleared when reuse of the previous token is detected.
    /// </remarks>
    /// 
    /// <author>Almighty-Shogun</author>
    /// <since>4.0.0</since>
    [MaxLength(64)]
    public string? PreviousRefreshTokenHash { get; set; }

    /// <summary>
    /// The concurrency token used to prevent conflicting session rotations.
    /// </summary>
    ///
    /// <remarks>
    /// Updated on rotation and persisted as an EF Core concurrency token.
    /// </remarks>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>4.0.0</since>
    internal Guid ConcurrencyToken { get; set; } = Guid.NewGuid();

    /// <summary>
    /// Indicates whether the session has expired.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>4.0.0</since>
    [NotMapped]
    public bool IsExpired => DateTimeOffset.UtcNow >= ExpiresAt;

    /// <summary>
    /// Indicates whether the session is neither revoked nor expired.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>4.0.0</since>
    [NotMapped]
    public bool IsActive => !IsRevoked && !IsExpired;
}
