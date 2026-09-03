using System.Text.Json.Serialization;
using System.Diagnostics.CodeAnalysis;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AlmightyShogun.AspNet.CredentialAuth;

/// <summary>
/// The user every credential service works against. An application inherits from it to add its own columns, and the
/// derived type becomes the <c>TUser</c> of the context and of every service, so there is only ever one user table.
/// </summary>
///
/// <author>Almighty-Shogun</author>
/// <since>Unreleased</since>
[Table("users")]
[SuppressMessage("ReSharper", "ClassCanBeSealed.Global")]
[SuppressMessage("ReSharper", "AutoPropertyCanBeMadeGetOnly.Global")]
public class AuthUser
{
    /// <summary>
    /// Gets or sets the primary key, which the other tables point at. Use <see cref="Identifier"/> in anything a client
    /// sees: this value is not hidden from serialization, so returning the entity exposes a sequential number.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    public int Id { get; set; }

    /// <summary>
    /// Gets or sets the identifier the outside world sees, carried in the <c>userId</c> claim and accepted by every
    /// service that takes a user. Version 7, so it still sorts by creation time and indexes without fragmenting.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    public Guid Identifier { get; set; } = Guid.CreateVersion7();

    /// <summary>
    /// Gets or sets the name the account signs in under, uniquely indexed and accepted by login alongside the address.
    /// Uniqueness is decided by the column's collation, so a case-sensitive one lets two accounts differ only in casing.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    [MaxLength(255)]
    public required string Username { get; set; }

    /// <summary>
    /// Gets or sets the address the account signs in under and the forgot-password flow matches on. Uniquely indexed,
    /// under the column's own collation.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    [MaxLength(255)]
    public required string Email { get; set; }

    /// <summary>
    /// Gets or sets the hash produced by ASP.NET Core's password hasher, never the password itself. Rehashed in place
    /// on sign-in when the hasher reports an outdated format, so raising the work factor takes effect as users return.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    [JsonIgnore]
    [MaxLength(255)]
    public string Password { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the refresh-token sessions opened against the account, one per signed-in device. Not loaded unless
    /// explicitly included, and ignored during JSON serialization so returning a user cannot leak its sessions.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    [JsonIgnore]
    [SuppressMessage("ReSharper", "CollectionNeverUpdated.Global")]
    public List<UserSession> Sessions { get; set; } = [];

    /// <summary>
    /// Gets or sets the single role written into the access token as a role claim. Settable like any other property, so
    /// never bind a client payload straight onto the entity.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    [MaxLength(255)]
    public string Role { get; set; } = "User";

    /// <summary>
    /// Gets or sets the permissions written into the access token, one claim each. Prefix them per application, as in
    /// <c>api:users.read</c>, only when routes are scoped that way; otherwise store the plain value.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    public string[] Permissions { get; set; } = [];

    /// <summary>
    /// Gets or sets whether the account may authenticate at all. Checked after the password, so refusing a disabled
    /// account cannot be used to discover which addresses are registered.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    public bool IsActive { get; set; } = true;

    /// <summary>
    /// Gets or sets the lockout state, or <c>null</c> while nothing has failed against the account. Held in its own
    /// table, so a deployment that leaves lockout disabled never writes one.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    public UserLockout? Lockout { get; set; }

    /// <summary>
    /// Gets or sets the two-factor enrolment, or <c>null</c> when the user has never enrolled. Not loaded with the user,
    /// so an ordinary read does not pull the secret along with it.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    public UserTwoFactor? TwoFactor { get; set; }
}
