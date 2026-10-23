using AlmightyShogun.AspNet.RequestValidation;

namespace AlmightyShogun.AspNet.Auth.Credentials;

/// <summary>
/// Represents a login request using a username or email address.
/// </summary>
///
/// <author>Almighty-Shogun</author>
/// <since>4.0.0</since>
public sealed record LoginRequest
{
    /// <summary>
    /// The username or email address used to sign in.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>4.0.0</since>
    [Required]
    public string Identifier { get; set; } = string.Empty;

    /// <summary>
    /// The password used to authenticate the user.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>4.0.0</since>
    [Required]
    public string Password { get; set; } = string.Empty;
}
