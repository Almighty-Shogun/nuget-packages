using AlmightyShogun.AspNet.Validation;

namespace AlmightyShogun.AspNet.CredentialAuth;

/// <summary>
/// Represents a request to start a forgot-password flow.
/// </summary>
///
/// <author>Almighty-Shogun</author>
/// <since>Unreleased</since>
public class ForgotPasswordRequest
{
    /// <summary>
    /// Gets or sets the email address for the password reset request.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    [Email]
    [Required]
    public required string Email { get; set; }
}
