using AlmightyShogun.AspNet.Validation;

namespace AlmightyShogun.AspNet.CredentialAuth;

/// <summary>
/// Represents a public registration request for a new credential user.
/// </summary>
///
/// <author>Almighty-Shogun</author>
/// <since>Unreleased</since>
public class RegisterRequest
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
    /// Gets or sets the plain-text password.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    [Min(8)]
    [Required]
    [PasswordSecure]
    public required string Password { get; set; }
}
