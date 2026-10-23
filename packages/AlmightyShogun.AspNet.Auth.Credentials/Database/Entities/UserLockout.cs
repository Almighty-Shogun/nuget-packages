using System.ComponentModel.DataAnnotations.Schema;

namespace AlmightyShogun.AspNet.Auth.Credentials;

/// <summary>
/// Tracks failed authentication attempts and account lockouts.
/// </summary>
///
/// <remarks>
/// Failed password and two-factor attempts share the same counter.
/// The record is removed after successful authentication.
/// </remarks>
///
/// <author>Almighty-Shogun</author>
/// <since>4.0.0</since>
[Table("user_lockouts")]
public sealed class UserLockout
{
    /// <summary>
    /// The lockout record's primary key.
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
    /// The number of failed authentication attempts.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>4.0.0</since>
    public int AccessFailedCount { get; set; }

    /// <summary>
    /// The lockout expiration timestamp, or null if not locked.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>4.0.0</since>
    public DateTimeOffset? LockoutEnd { get; set; }

    /// <summary>
    /// Indicates whether the account is currently locked.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>4.0.0</since>
    [NotMapped]
    public bool IsLocked => LockoutEnd is not null && LockoutEnd > DateTimeOffset.UtcNow;
}
