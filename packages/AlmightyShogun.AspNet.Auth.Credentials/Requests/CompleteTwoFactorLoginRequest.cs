using AlmightyShogun.AspNet.RequestValidation;

namespace AlmightyShogun.AspNet.Auth.Credentials;

/// <summary>
/// Represents a request to complete a two-factor login.
/// </summary>
///
/// <author>Almighty-Shogun</author>
/// <since>4.1.0</since>
public sealed record CompleteTwoFactorLoginRequest
{
    /// <summary>
    /// The challenge token issued during sign-in.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>4.1.0</since>
    [Required]
    public string Challenge { get; set; } = string.Empty;

    /// <summary>
    /// The authenticator or recovery code used to complete login.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>4.1.0</since>
    [Required]
    public string Code { get; set; } = string.Empty;
}
