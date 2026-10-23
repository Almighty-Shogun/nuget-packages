using AlmightyShogun.AspNet.RequestValidation;

namespace AlmightyShogun.AspNet.Auth.Credentials;

/// <summary>
/// Represents a public user registration request.
/// </summary>
///
/// <author>Almighty-Shogun</author>
/// <since>4.0.0</since>
public sealed record RegisterRequest
{
    /// <summary>
    /// The new user's username.
    /// </summary>
    ///
    /// <remarks>
    /// Does not allow assigning roles or permissions.
    /// Use <see cref="CreateUserRequest"/> for administrative user creation.
    /// </remarks>
    /// 
    /// <author>Almighty-Shogun</author>
    /// <since>4.0.0</since>
    [Required]
    public string Username { get; set; } = string.Empty;

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
    /// The new user's password.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>4.0.0</since>
    [Min(8)]
    [Required]
    [PasswordSecure]
    public string Password { get; set; } = string.Empty;
}
