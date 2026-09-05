using AlmightyShogun.AspNet.RequestValidation;

namespace AlmightyShogun.AspNet.Auth.Credentials;

/// <summary>
/// A password change for the signed-in user. The service re-checks the confirmation, the current password, and reuse, but
/// not the length and strength rules below, so an application posting its own shape has to enforce those itself.
/// </summary>
///
/// <author>Almighty-Shogun</author>
/// <since>4.0.0</since>
public sealed class ChangePasswordRequest
{
    /// <summary>
    /// The password the account currently signs in with, which is the one being replaced.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>4.0.0</since>
    [Required]
    public required string CurrentPassword { get; set; }

    /// <summary>
    /// The password the account should sign in with afterwards.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>4.0.0</since>
    [Min(8)]
    [Required]
    [PasswordSecure]
    public required string NewPassword { get; set; }

    /// <summary>
    /// The repeat of the new password, which catches a typo before it becomes a credential nobody knows.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>4.0.0</since>
    [Min(8)]
    [Required]
    public required string ConfirmPassword { get; set; }
}
