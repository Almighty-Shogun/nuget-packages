using AlmightyShogun.AspNet.RequestValidation;

namespace AlmightyShogun.AspNet.Auth.Credentials;

/// <summary>
/// An administrative user creation, where the caller is trusted to assign authorization. Use
/// <see cref="RegisterRequest"/> for public sign-up, which deliberately carries no role or permissions.
/// </summary>
///
/// <author>Almighty-Shogun</author>
/// <since>Unreleased</since>
public class CreateUserRequest
{
    /// <summary>
    /// Gets or sets the username to claim. Refused when another account already holds it.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    [Required]
    public required string Username { get; set; }

    /// <summary>
    /// Gets or sets the initial password, hashed before it is stored and never persisted as given.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    [Min(8)]
    [Required]
    [PasswordSecure]
    public required string Password { get; set; }

    /// <summary>
    /// Gets or sets the email address to claim. Refused when another account already holds it.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    [Email]
    [Required]
    public required string Email { get; set; }

    /// <summary>
    /// Gets or sets the role written into the user's tokens. Trusted from the caller, which is why this request is for
    /// administrative use only.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    public string Role { get; set; } = "User";

    /// <summary>
    /// Gets or sets the permissions written into the user's tokens, one claim each. Trusted from the caller, so a public
    /// endpoint must never bind to this model.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    public string[] Permissions { get; set; } = [];
}
