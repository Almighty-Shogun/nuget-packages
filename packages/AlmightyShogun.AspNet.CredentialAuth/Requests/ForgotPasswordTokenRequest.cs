using AlmightyShogun.AspNet.Validation;

namespace AlmightyShogun.AspNet.CredentialAuth;

/// <summary>
/// Represents a request to validate a forgot-password token.
/// </summary>
///
/// <author>Almighty-Shogun</author>
/// <since>Unreleased</since>
public class ForgotPasswordTokenRequest
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
}
