using AlmightyShogun.AspNet.RequestValidation;

namespace AlmightyShogun.AspNet.Auth.Credentials;

/// <summary>
/// Represents a request to reset a password using a recovery token.
/// </summary>
///
/// <author>Almighty-Shogun</author>
/// <since>4.0.0</since>
public sealed record CompleteForgotPasswordRequest
{
    /// <summary>
    /// The password reset token.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>4.0.0</since>
    [Required]
    public string Token { get; set; } = string.Empty;

    /// <summary>
    /// The new password.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>4.0.0</since>
    [Min(8)]
    [Required]
    [PasswordSecure]
    public string NewPassword { get; set; } = string.Empty;

    /// <summary>
    /// Confirmation of the new password.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>4.0.0</since>
    [Min(8)]
    [Required]
    [SameAs(nameof(NewPassword))]
    public string ConfirmPassword { get; set; } = string.Empty;
}
