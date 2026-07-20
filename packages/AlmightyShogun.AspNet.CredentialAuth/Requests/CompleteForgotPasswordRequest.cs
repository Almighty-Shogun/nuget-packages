using AlmightyShogun.AspNet.Validation;

namespace AlmightyShogun.AspNet.CredentialAuth;

/// <summary>
/// Represents a request to complete a forgot-password flow.
/// </summary>
///
/// <author>Almighty-Shogun</author>
/// <since>Unreleased</since>
public class CompleteForgotPasswordRequest
{
    /// <summary>
    /// Gets or sets the password reset token.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    [Required]
    [PasswordResetToken]
    public required string Token { get; set; }

    /// <summary>
    /// Gets or sets the new password.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    [Min(8)]
    [Required]
    [PasswordSecure]
    [NotCurrentPassword]
    public required string NewPassword { get; set; }

    /// <summary>
    /// Gets or sets the new password confirmation.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    [Min(8)]
    [Required]
    [PasswordMatch]
    public required string ConfirmPassword { get; set; }
}
