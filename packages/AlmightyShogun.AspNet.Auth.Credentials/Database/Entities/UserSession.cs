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
    /// Gets or sets when this row was inserted, which is the sign-in that opened it rather than a first sign-in on the
    /// device. A configured absolute lifetime is measured from here, so refreshing cannot carry the session past it.
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
    /// Gets or sets whether the session has been ended, by a sign-out, by a password change, or by a detected replay of a
    /// spent refresh token, which sets it on every live session the user holds. The row is kept rather than deleted, and a
    /// refresh presented against it is refused exactly as an unknown token is.
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
    [MaxLength(256)]
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
    /// Gets or sets the value that guards a rotation against a concurrent one. Rewritten on every rotation and mapped as
    /// the row's concurrency token, so two refreshes that read the same session leave only the first one's write standing
    /// and the second fails with <see cref="DbUpdateConcurrencyException"/>.
    /// </summary>
    ///
    /// <remarks>
    /// It is a real column despite being internal, so an application's migration has to create it.
    /// </remarks>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    internal Guid ConcurrencyToken { get; set; } = Guid.NewGuid();

    /// <summary>
    /// Gets whether the session is past its expiry, computed rather than stored so it needs no sweep to stay accurate.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    [NotMapped]
    public bool IsExpired => DateTimeOffset.UtcNow >= ExpiresAt;

    /// <summary>
    /// Gets whether a refresh presented against this session would be honored, which is neither revoked nor expired.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    [NotMapped]
    public bool IsActive => !IsRevoked && !IsExpired;
}
