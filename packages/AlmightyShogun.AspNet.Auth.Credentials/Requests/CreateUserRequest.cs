using AlmightyShogun.AspNet.RequestValidation;

namespace AlmightyShogun.AspNet.Auth.Credentials;

/// <summary>
/// An administrative user creation, where the caller is trusted to assign authorization. Use
/// <see cref="RegisterRequest"/> for public sign-up, which deliberately carries no role or permissions.
/// </summary>
///
/// <author>Almighty-Shogun</author>
/// <since>4.0.0</since>
public sealed record CreateUserRequest
{
    /// <summary>
    /// The username to claim. Refused when another account already holds it.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>4.0.0</since>
    [Required]
    public string Username { get; set; } = string.Empty;

    /// <summary>
    /// The initial password, hashed before it is stored and never persisted as given.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>4.0.0</since>
    [Min(8)]
    [Required]
    [PasswordSecure]
    public string Password { get; set; } = string.Empty;

    /// <summary>
    /// The email address to claim. Refused when another account already holds it.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>4.0.0</since>
    [Email]
    [Required]
    public string Email { get; set; } = string.Empty;

    /// <summary>
    /// The role written into the user's tokens. Trusted from the caller, which is why this request is for
    /// administrative use only.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>4.0.0</since>
    public string Role { get; set; } = "User";

    /// <summary>
    /// The permissions written into the user's tokens, one claim each. Trusted from the caller, so a public
    /// endpoint must never bind to this model.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>4.0.0</since>
    public string[] Permissions { get; set; } = [];
}
