using AlmightyShogun.AspNet.Validation;

namespace AlmightyShogun.AspNet.CredentialAuth;

/// <summary>
/// Represents a request to create a new authentication user.
/// </summary>
///
/// <author>Almighty-Shogun</author>
/// <since>Unreleased</since>
public class CreateUserRequest
{
    /// <summary>
    /// Gets or sets the unique username.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    [Required]
    [UniqueUsername]
    public required string Username { get; set; }

    /// <summary>
    /// Gets or sets the plain-text password.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    [Min(8)]
    [Required]
    [PasswordSecure]
    public required string Password { get; set; }

    /// <summary>
    /// Gets or sets the unique email address.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    [Email]
    [Required]
    [UniqueEmail]
    public required string Email { get; set; }

    /// <summary>
    /// Gets or sets the role assigned to the user.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    public string Role { get; set; } = "User";

    /// <summary>
    /// Gets or sets the permissions assigned to the user.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    public string[] Permissions { get; set; } = [];
}
