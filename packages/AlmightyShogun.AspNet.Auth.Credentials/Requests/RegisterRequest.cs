using AlmightyShogun.AspNet.RequestValidation;

namespace AlmightyShogun.AspNet.Auth.Credentials;

/// <summary>
/// A public sign-up. Deliberately carries no role or permissions, so a client cannot grant itself authorization; use
/// <see cref="CreateUserRequest"/> where the caller is trusted to assign them.
/// </summary>
///
/// <author>Almighty-Shogun</author>
/// <since>Unreleased</since>
public sealed record RegisterRequest
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
    /// Gets or sets the email address to claim. Refused when another account already holds it.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    [Email]
    [Required]
    public required string Email { get; set; }

    /// <summary>
    /// Gets or sets the chosen password, hashed before it is stored and never persisted as given.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    [Min(8)]
    [Required]
    [PasswordSecure]
    public required string Password { get; set; }
}
