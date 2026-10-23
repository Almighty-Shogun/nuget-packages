using System.Diagnostics.CodeAnalysis;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AlmightyShogun.AspNet.Auth.Credentials;

/// <summary>
/// Base user entity for credential authentication.
/// </summary>
///
/// <remarks>
/// Applications can inherit from this class to extend the user entity.
/// </remarks>
/// 
/// <author>Almighty-Shogun</author>
/// <since>4.0.0</since>
[Table("users")]
[SuppressMessage("ReSharper", "ClassCanBeSealed.Global")]
[SuppressMessage("ReSharper", "AutoPropertyCanBeMadeGetOnly.Global")]
public class AuthUser
{
    /// <summary>
    /// The internal primary key.
    /// </summary>
    ///
    /// <remarks>
    /// Use <see cref="Identifier"/> for public user identification.
    /// </remarks>
    /// 
    /// <author>Almighty-Shogun</author>
    /// <since>4.0.0</since>
    public int Id { get; set; }

    /// <summary>
    /// The public user identifier, generated as a version 7 GUID.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>4.0.0</since>
    public Guid Identifier { get; set; } = Guid.CreateVersion7();

    /// <summary>
    /// The unique username used for authentication.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>4.0.0</since>
    [MaxLength(255)]
    public required string Username { get; set; }

    /// <summary>
    /// The unique email address used for authentication and account recovery.
    /// </summary>
    ///
    /// <remarks>
    /// Changing a verified email requires resetting <see cref="EmailVerifiedAt"/>
    /// or using <see cref="IAuthEmailService.CompleteEmailChangeAsync"/>.
    /// </remarks>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>4.0.0</since>
    [MaxLength(255)]
    public required string Email { get; set; }

    /// <summary>
    /// The email verification timestamp, or null if unverified.
    /// </summary>
    ///
    /// <remarks>
    /// Email verification is not enforced automatically during authentication.
    /// </remarks>
    /// 
    /// <author>Almighty-Shogun</author>
    /// <since>4.1.0</since>
    public DateTimeOffset? EmailVerifiedAt { get; set; }

    /// <summary>
    /// The hashed password.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>4.0.0</since>
    [MaxLength(255)]
    public string Password { get; set; } = string.Empty;

    /// <summary>
    /// The user's refresh token sessions.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>4.0.0</since>
    [SuppressMessage("ReSharper", "CollectionNeverUpdated.Global")]
    public List<UserSession> Sessions { get; set; } = [];

    /// <summary>
    /// The user's role included in access token claims.
    /// </summary>
    ///
    /// <remarks>
    /// Do not allow clients to assign roles directly.
    /// </remarks>
    /// 
    /// <author>Almighty-Shogun</author>
    /// <since>4.0.0</since>
    [MaxLength(255)]
    public string Role { get; set; } = "User";

    /// <summary>
    /// The user's permissions included in access token claims.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>4.0.0</since>
    public string[] Permissions { get; set; } = [];

    /// <summary>
    /// Indicates whether the account is active.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>4.0.0</since>
    public bool IsActive { get; set; } = true;

    /// <summary>
    /// The user's lockout state, if loaded.
    /// </summary>
    ///
    /// <remarks>
    /// A null navigation does not necessarily mean the user is not locked out.
    /// </remarks>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>4.0.0</since>
    public UserLockout? Lockout { get; set; }

    /// <summary>
    /// The user's two-factor enrollment, if loaded.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>4.0.0</since>
    public UserTwoFactor? TwoFactor { get; set; }
}
