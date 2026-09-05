using AlmightyShogun.AspNet.RequestValidation;

namespace AlmightyShogun.AspNet.Auth.Credentials;

/// <summary>
/// The second half of a forgot-password flow, exchanging a reset token for a new password. The token is spent on
/// success, so the same one cannot set the password twice. The service re-checks the token, the confirmation, and reuse,
/// but not the length and strength rules below, so an application posting its own shape has to enforce those itself.
/// </summary>
///
/// <author>Almighty-Shogun</author>
/// <since>Unreleased</since>
public sealed record CompleteForgotPasswordRequest
{
    /// <summary>
    /// The token from the reset email, in the form it was sent rather than a decoded or trimmed one.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    [Required]
    public required string Token { get; set; }

    /// <summary>
    /// The password the account should sign in with afterwards.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    [Min(8)]
    [Required]
    [PasswordSecure]
    public required string NewPassword { get; set; }

    /// <summary>
    /// The repeat of the new password, which catches a typo before it becomes a credential nobody knows.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    [Min(8)]
    [Required]
    public required string ConfirmPassword { get; set; }
}
