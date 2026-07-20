using AlmightyShogun.AspNet.Validation;

namespace AlmightyShogun.AspNet.CredentialAuth;

/// <summary>
/// Represents a request to change the current authenticated user's password.
/// </summary>
///
/// <author>Almighty-Shogun</author>
/// <since>Unreleased</since>
public class ChangePasswordRequest
{
    /// <summary>
    /// Gets or sets the user's current password.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    [Required]
    [CurrentPassword]
    public required string CurrentPassword { get; set; }

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
