using AlmightyShogun.AspNet.RequestValidation;

namespace AlmightyShogun.AspNet.Auth.Credentials;

/// <summary>
/// The second half of a forgot-password flow, exchanging a reset token for a new password. The token is spent on
/// success, so the same one cannot set the password twice.
/// </summary>
///
/// <author>Almighty-Shogun</author>
/// <since>Unreleased</since>
public sealed record CompleteForgotPasswordRequest
{
    /// <summary>
    /// Gets or sets the token from the reset email, in the form it was sent. Only its hash is stored, so it is matched by
    /// hashing what arrives rather than by looking the value up.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    [Required]
    public required string Token { get; set; }

    /// <summary>
    /// Gets or sets the replacement. Refused when it matches the password already in use.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    [Min(8)]
    [Required]
    [PasswordSecure]
    public required string NewPassword { get; set; }

    /// <summary>
    /// Gets or sets the repeat of the new password, refused when the two differ.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    [Min(8)]
    [Required]
    public required string ConfirmPassword { get; set; }
}
