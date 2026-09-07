using System.Diagnostics.CodeAnalysis;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AlmightyShogun.AspNet.Auth.Credentials;

/// <summary>
/// One sign-in that got past the password and is waiting on a second factor. <see cref="IAuthUserService{TUser}"/>
/// writes a row when it refuses to open a session, and spends it once a code verifies; a spent row is kept rather than
/// deleted, so presenting the same challenge twice is refused exactly as an expired one is.
/// </summary>
///
/// <remarks>
/// A row is cleared out only once it has expired, and only as the same user is issued another challenge, so the table
/// keeps a record of the sign-ins an account has recently owed a code for.
/// </remarks>
///
/// <author>Almighty-Shogun</author>
/// <since>4.1.0</since>
[Table("two_factor_challenges")]
[SuppressMessage("ReSharper", "PropertyCanBeMadeInitOnly.Global")]
public sealed class TwoFactorChallenge
{
    /// <summary>
    /// The surrogate key, which completion names when it claims the row it just read. Nothing hands it to a client: the
    /// challenge is the only handle a caller has on this row.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>4.1.0</since>
    public int Id { get; set; }

    /// <summary>
    /// The user whose password verified. Cascades with the user, so removing an account takes its outstanding challenges
    /// with it. Not unique: a spent row stays beside the live one.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>4.1.0</since>
    public int UserId { get; set; }

    /// <summary>
    /// The hash of the challenge that was returned, so the value the client holds cannot be read back out of the table.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>4.1.0</since>
    [Required]
    [MaxLength(64)]
    public string TokenHash { get; set; } = string.Empty;

    /// <summary>
    /// The application the sign-in came through, or <c>null</c> when the deployment is not app-scoped. Completion
    /// resolves its own request's host and refuses a challenge whose value differs, so the code has to be presented to
    /// the application the password was sent to and the session ends up scoped to it.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>4.1.0</since>
    [MaxLength(255)]
    public string? App { get; set; }

    /// <summary>
    /// When the password verified and this challenge was issued, set as the row is written and never changed afterwards.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>4.1.0</since>
    public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;

    /// <summary>
    /// When the challenge stops being redeemable, set at issue from <see cref="TwoFactorPolicy.ChallengeMinutes"/>. It
    /// bounds how long the code prompt may sit open, not how long the session it leads to lasts.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>4.1.0</since>
    public DateTimeOffset ExpiresAt { get; set; }

    /// <summary>
    /// When the challenge was spent, or <c>null</c> while it is still redeemable. Stamped when a code completes the
    /// sign-in, when a later sign-in for the same user retires it, so only the most recent one is ever live, and when
    /// <see cref="IAuthPasswordService"/> sets a new password and retires whatever the old one bought.
    /// </summary>
    ///
    /// <remarks>
    /// The first two stamps are written by an update statement that bypasses the change tracker, so the row in the
    /// database carries the new value while an instance loaded beforehand keeps the one it was read with. The password
    /// paths stamp loaded instances instead and leave the write to the save that follows.
    /// </remarks>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>4.1.0</since>
    public DateTimeOffset? UsedAt { get; set; }

    /// <summary>
    /// The address the password was sent from, kept for auditing a sign-in that stopped at the second factor. Trimmed to 45
    /// characters at the write, which holds any IPv6 address in its longest plain form but not one carrying a scope id.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>4.1.0</since>
    [MaxLength(45)]
    public string? RequestedIpAddress { get; set; }

    /// <summary>
    /// Whether the challenge would still be accepted, which is unspent and not past its expiry.
    /// </summary>
    ///
    /// <remarks>
    /// Completion tests these two conditions in the database rather than reading this property, and claims the row with
    /// a guarded update, so an active row is not necessarily one that request will get to spend. This also answers for
    /// the instance rather than for the row: <see cref="UsedAt"/> is stamped past the change tracker, so one loaded
    /// before that write still reports itself unspent until it is read again.
    /// </remarks>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>4.1.0</since>
    [NotMapped]
    public bool IsActive => UsedAt is null && ExpiresAt > DateTimeOffset.UtcNow;
}
