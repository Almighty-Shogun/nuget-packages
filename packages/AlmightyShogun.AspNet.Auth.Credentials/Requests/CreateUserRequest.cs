using AlmightyShogun.AspNet.RequestValidation;

namespace AlmightyShogun.AspNet.Auth.Credentials;

/// <summary>
/// Represents an administrative request to create a user with assigned authorization.
/// </summary>
///
/// <remarks>
/// Use <see cref="RegisterRequest"/> for public registration.
/// Do not expose this model through public registration endpoints.
/// </remarks>
/// 
/// <author>Almighty-Shogun</author>
/// <since>4.0.0</since>
public sealed record CreateUserRequest
{
    /// <summary>
    /// The new user's username.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>4.0.0</since>
    [Required]
    public string Username { get; set; } = string.Empty;

    /// <summary>
    /// The new user's initial password.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>4.0.0</since>
    [Min(8)]
    [Required]
    [PasswordSecure]
    public string Password { get; set; } = string.Empty;

    /// <summary>
    /// The new user's email address.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>4.0.0</since>
    [Email]
    [Required]
    public string Email { get; set; } = string.Empty;

    /// <summary>
    /// The role assigned to the user.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>4.0.0</since>
    public string Role { get; set; } = "User";

    /// <summary>
    /// The permissions assigned to the user.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>4.0.0</since>
    public string[] Permissions { get; set; } = [];
}
