using Microsoft.EntityFrameworkCore;
using System.Diagnostics.CodeAnalysis;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AlmightyShogun.AspNet.Auth.Credentials;

/// <summary>
/// One signed-in session, keyed by the refresh token it holds. A row is inserted per sign-in, with nothing matching it
/// against the device presenting it, so signing in twice from one browser leaves two rows and ending either leaves the
/// other usable.
/// </summary>
///
/// <author>Almighty-Shogun</author>
/// <since>4.0.0</since>
[Table("user_sessions")]
[SuppressMessage("ReSharper", "PropertyCanBeMadeInitOnly.Global")]
public sealed class UserSession
{
    /// <summary>
    /// The surrogate key. Never handed to a client: the refresh token is the only handle anyone outside
    /// the application has on a session.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>4.0.0</since>
    public int Id { get; set; }

    /// <summary>
    /// The user this session belongs to. Cascades, so deleting a user takes their sessions with it.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>4.0.0</since>
    public int UserId { get; set; }

    /// <summary>
    /// The hash of the current refresh token. Hashed rather than stored, so a database copy cannot be used to
    /// resume anyone's session.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>4.0.0</since>
    [Required]
    [MaxLength(64)]
    public string RefreshTokenHash { get; set; } = string.Empty;

    /// <summary>
    /// The application the session belongs to, or <c>null</c> when the deployment is not app-scoped. Written
    /// from the host the sign-in came through and never changed by a refresh.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>4.0.0</since>
    [MaxLength(255)]
    public string? App { get; set; }

    /// <summary>
    /// When the session stops being usable. Extended on each refresh, up to the absolute lifetime.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>4.0.0</since>
    public DateTimeOffset ExpiresAt { get; set; }

    /// <summary>
    /// When this row was inserted, which is the sign-in that opened it rather than a first sign-in on the
    /// device. A configured absolute lifetime is measured from here, so refreshing cannot carry the session past it.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>4.0.0</since>
    public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;

    /// <summary>
    /// When the session was last refreshed, both for showing a user their devices and for deciding whether a
    /// replayed token falls inside the rotation grace.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>4.0.0</since>
    public DateTimeOffset LastActiveAt { get; set; } = DateTimeOffset.UtcNow;

    /// <summary>
    /// Whether the session has been ended. Ending one sets this rather than deleting the row, so the session
    /// stays visible to a query afterwards.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>4.0.0</since>
    public bool IsRevoked { get; set; }

    /// <summary>
    /// The address the session was last used from, for showing a user where they are signed in. Trimmed to 45 characters at the
    /// write, which holds any IPv6 address in its longest plain form but not one carrying a scope id, and not a chain of
    /// forwarded addresses.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>4.0.0</since>
    [MaxLength(45)]
    public string? IpAddress { get; set; }

    /// <summary>
    /// The raw user agent, kept alongside the parsed fields so an unrecognised client is still identifiable. Trimmed to 512
    /// characters at the write, since the header arrives at whatever length the client chose to send.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>4.0.0</since>
    [MaxLength(512)]
    public string? UserAgent { get; set; }

    /// <summary>
    /// The device parsed from the user agent, or <c>null</c> when it could not be determined. Trimmed to 256 characters at the
    /// write.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>4.0.0</since>
    [MaxLength(256)]
    public string? Device { get; set; }

    /// <summary>
    /// The browser parsed from the user agent, or <c>null</c> when it could not be determined. Trimmed to 256 characters at the
    /// write.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>4.0.0</since>
    [MaxLength(256)]
    public string? Browser { get; set; }

    /// <summary>
    /// The operating system parsed from the user agent, or <c>null</c> when it could not be determined. Trimmed to 256
    /// characters at the write.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>4.0.0</since>
    [MaxLength(256)]
    public string? Os { get; set; }

    /// <summary>
    /// The hash of the refresh token this session replaced, or <c>null</c> before its first rotation and again once a
    /// replay of it has been treated as theft. A request presenting it is a replay of a spent token, which is the signal
    /// that a refresh token has been stolen; clearing it as that fires keeps one retired token to a single detection.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>4.0.0</since>
    [MaxLength(64)]
    public string? PreviousRefreshTokenHash { get; set; }

    /// <summary>
    /// The value that guards a rotation against a concurrent one. Rewritten on every rotation and mapped as
    /// the row's concurrency token, so two refreshes that read the same session leave only the first one's write standing
    /// and the second fails with <see cref="DbUpdateConcurrencyException"/>.
    /// </summary>
    ///
    /// <remarks>
    /// It is a real column despite being internal, so an application's migration has to create it.
    /// </remarks>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>4.0.0</since>
    internal Guid ConcurrencyToken { get; set; } = Guid.NewGuid();

    /// <summary>
    /// Whether the session is past its expiry, computed rather than stored so it needs no sweep to stay accurate.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>4.0.0</since>
    [NotMapped]
    public bool IsExpired => DateTimeOffset.UtcNow >= ExpiresAt;

    /// <summary>
    /// Whether a refresh presented against this session would be honored, which is neither revoked nor expired.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>4.0.0</since>
    [NotMapped]
    public bool IsActive => !IsRevoked && !IsExpired;
}
