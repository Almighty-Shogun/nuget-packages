using System.Diagnostics.CodeAnalysis;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AlmightyShogun.AspNet.Auth.Credentials;

/// <summary>
/// The user every credential service works against. An application inherits from it to add its own columns, and the
/// derived type becomes the <c>TUser</c> of the context and of every service, so there is only ever one user table.
/// </summary>
///
/// <author>Almighty-Shogun</author>
/// <since>4.0.0</since>
[Table("users")]
[SuppressMessage("ReSharper", "ClassCanBeSealed.Global")]
[SuppressMessage("ReSharper", "AutoPropertyCanBeMadeGetOnly.Global")]
public class AuthUser
{
    /// <summary>
    /// The primary key, which the other tables point at. Use <see cref="Identifier"/> in anything a client
    /// sees: this value is not hidden from serialization, so returning the entity exposes a sequential number.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>4.0.0</since>
    public int Id { get; set; }

    /// <summary>
    /// The identifier the outside world sees, carried in the <c>userId</c> claim and accepted by every
    /// service that takes a user. Version 7, so it still sorts by creation time and indexes without fragmenting.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>4.0.0</since>
    public Guid Identifier { get; set; } = Guid.CreateVersion7();

    /// <summary>
    /// The name the account signs in under, uniquely indexed and accepted by login alongside the address.
    /// Uniqueness is decided by the column's collation, so a case-sensitive one lets two accounts differ only in casing.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>4.0.0</since>
    [MaxLength(255)]
    public required string Username { get; set; }

    /// <summary>
    /// The address the account signs in under and the forgot-password flow matches on. Uniquely indexed,
    /// under the column's own collation.
    /// </summary>
    ///
    /// <remarks>
    /// Writing this on a user whose <see cref="EmailVerifiedAt"/> already carries a value leaves that timestamp describing
    /// an address nobody confirmed, since nothing intercepts the write. Move a verified address through
    /// <see cref="IAuthEmailService.CompleteEmailChangeAsync"/>, or clear the timestamp in the same save. Setting the
    /// address on a user who has never verified one, as creating an account does, needs neither: the timestamp is null and
    /// reads as unverified.
    /// </remarks>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>4.0.0</since>
    [MaxLength(255)]
    public required string Email { get; set; }

    /// <summary>
    /// When the account's address was last proved, or <c>null</c> while it never has been. Recorded and never acted on:
    /// nothing in the package refuses an unverified account, so whether one may sign in is the application's own check.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    public DateTimeOffset? EmailVerifiedAt { get; set; }

    /// <summary>
    /// The hash produced by ASP.NET Core's password hasher, never the password itself. Empty on a new entity
    /// until one of the credential services writes a hash into it.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>4.0.0</since>
    [MaxLength(255)]
    public string Password { get; set; } = string.Empty;

    /// <summary>
    /// The refresh-token sessions opened against the account. Not loaded unless explicitly included, and see
    /// <see cref="UserSession"/> for what one row stands for.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>4.0.0</since>
    [SuppressMessage("ReSharper", "CollectionNeverUpdated.Global")]
    public List<UserSession> Sessions { get; set; } = [];

    /// <summary>
    /// The single role written into the access token as a role claim. Settable like any other property, so
    /// never bind a client payload straight onto the entity.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>4.0.0</since>
    [MaxLength(255)]
    public string Role { get; set; } = "User";

    /// <summary>
    /// The permissions written into the access token, one claim each. Prefix them per application, as in
    /// <c>api:users.read</c>, only when routes are scoped that way; otherwise store the plain value.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>4.0.0</since>
    public string[] Permissions { get; set; } = [];

    /// <summary>
    /// Whether the account may authenticate at all. Clearing it deactivates the account without deleting the
    /// row or its credentials, so setting it again restores the account as it was.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>4.0.0</since>
    public bool IsActive { get; set; } = true;

    /// <summary>
    /// The lockout state, held in its own table so a deployment that leaves lockout disabled never writes
    /// one. Nothing in the package loads this navigation, reading that table directly instead, so it is populated only when
    /// the change tracker happens to hold a matching row from the same context, as it does after a lockout check on the
    /// login, two-factor, and refresh paths. Do not read it as a reliable answer to whether a lockout exists.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>4.0.0</since>
    public UserLockout? Lockout { get; set; }

    /// <summary>
    /// The two-factor enrolment, or <c>null</c> when the user has never enrolled. Not loaded with the user,
    /// so an ordinary read does not pull the secret along with it.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>4.0.0</since>
    public UserTwoFactor? TwoFactor { get; set; }
}
