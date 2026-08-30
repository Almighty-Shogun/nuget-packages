using System.Text.Json.Serialization;
using System.Diagnostics.CodeAnalysis;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AlmightyShogun.AspNet.CredentialAuth;

/// <summary>
/// One device's signed-in session, keyed by the refresh token it holds. A user has one row per device rather than one
/// overall, so a sign-out on a phone leaves a laptop signed in.
/// </summary>
///
/// <author>Almighty-Shogun</author>
/// <since>Unreleased</since>
[Table("user_sessions")]
[SuppressMessage("ReSharper", "PropertyCanBeMadeInitOnly.Global")]
public sealed class UserSession
{
    /// <summary>
    /// Gets or sets the surrogate key. Never handed to a client: the refresh token is the only handle anyone outside
    /// the application has on a session.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    public int Id { get; set; }

    /// <summary>
    /// Gets or sets the user this session belongs to. Cascades, so deleting a user takes their sessions with it.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    public int UserId { get; set; }

    /// <summary>
    /// Gets or sets the hash of the current refresh token. Hashed rather than stored, so a database copy cannot be used to
    /// resume anyone's session.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    [Required]
    [JsonIgnore]
    [MaxLength(64)]
    public string RefreshTokenHash { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the application the session belongs to, or <c>null</c> when the deployment is not app-scoped. A refresh
    /// presented against a different application is refused.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    [MaxLength(255)]
    public string? App { get; set; }

    /// <summary>
    /// Gets or sets when the session stops being usable. Extended on each refresh, up to the absolute lifetime.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    public DateTimeOffset ExpiresAt { get; set; }

    /// <summary>
    /// Gets or sets when the user first signed in on this device, which bounds the absolute lifetime.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;

    /// <summary>
    /// Gets or sets when the session was last refreshed, both for showing a user their devices and for deciding whether a
    /// replayed token falls inside the rotation grace.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    public DateTimeOffset LastActiveAt { get; set; } = DateTimeOffset.UtcNow;

    /// <summary>
    /// Gets or sets whether the session was ended deliberately, by a sign-out or a password change. Kept rather than
    /// deleted, so a replay of its token is recognised as one.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    public bool IsRevoked { get; set; }

    /// <summary>
    /// Gets or sets the address the session was last used from, for showing a user where they are signed in.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    [MaxLength(45)]
    public string? IpAddress { get; set; }

    /// <summary>
    /// Gets or sets the raw user agent, kept alongside the parsed fields so an unrecognised client is still identifiable.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    [MaxLength(512)]
    public string? UserAgent { get; set; }

    /// <summary>
    /// Gets or sets the device parsed from the user agent, or <c>null</c> when it could not be determined.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    [MaxLength(256)]
    public string? Device { get; set; }

    /// <summary>
    /// Gets or sets the browser parsed from the user agent, or <c>null</c> when it could not be determined.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    [MaxLength(256)]
    public string? Browser { get; set; }

    /// <summary>
    /// Gets or sets the operating system parsed from the user agent, or <c>null</c> when it could not be determined.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    [MaxLength(255)]
    public string? Os { get; set; }

    /// <summary>
    /// Gets or sets the hash of the refresh token this session replaced, or <c>null</c> before its first rotation. A
    /// request presenting it is a replay of a spent token, which is the signal that a refresh token has been stolen.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    [MaxLength(64)]
    public string? PreviousRefreshTokenHash { get; set; }

    /// <summary>
    /// Gets whether the session is past its expiry, computed rather than stored so it needs no sweep to stay accurate.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    [NotMapped]
    public bool IsExpired => DateTimeOffset.UtcNow >= ExpiresAt;

    /// <summary>
    /// Gets whether a refresh presented against this session would be honoured, which is neither revoked nor expired.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    [NotMapped]
    public bool IsActive => !IsRevoked && !IsExpired;
}
