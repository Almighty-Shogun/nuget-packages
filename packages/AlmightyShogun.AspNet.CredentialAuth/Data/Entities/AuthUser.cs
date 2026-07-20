using System.Text.Json.Serialization;
using System.Diagnostics.CodeAnalysis;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AlmightyShogun.AspNet.CredentialAuth;

/// <summary>
/// Represents the base authentication user entity.
/// </summary>
///
/// <author>Almighty-Shogun</author>
/// <since>Unreleased</since>
[Table("users")]
public class AuthUser
{
    /// <summary>
    /// Gets or sets the user identifier.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    public int Id { get; set; }

    /// <summary>
    /// Gets or sets the unique username.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    [MaxLength(50)]
    public required string Username { get; set; }

    /// <summary>
    /// Gets or sets the unique email address.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    [MaxLength(255)]
    public required string Email { get; set; }

    /// <summary>
    /// Gets or sets the hashed password.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    [JsonIgnore]
    [MaxLength(255)]
    public string Password { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the sessions owned by the user.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    [JsonIgnore]
    [SuppressMessage("ReSharper", "CollectionNeverUpdated.Global")]
    public List<UserSession> Sessions { get; set; } = [];

    /// <summary>
    /// Gets or sets the user role.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    [MaxLength(255)]
    public string Role { get; set; } = "User";

    /// <summary>
    /// Gets or sets the permission names assigned to the user.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    public string[] Permissions { get; set; } = [];
}
