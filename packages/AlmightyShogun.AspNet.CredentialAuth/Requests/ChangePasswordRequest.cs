using AlmightyShogun.AspNet.RequestValidation;

namespace AlmightyShogun.AspNet.CredentialAuth;

/// <summary>
/// A password change for the signed-in user. Every field is checked again by the service, so an application posting its
/// own shape gets the same guarantees as one binding this model.
/// </summary>
///
/// <author>Almighty-Shogun</author>
/// <since>Unreleased</since>
public class ChangePasswordRequest
{
    /// <summary>
    /// Gets or sets the password being replaced. Verified against the stored hash, so a change cannot be made from a
    /// session alone if the password itself is unknown.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    [Required]
    public required string CurrentPassword { get; set; }

    /// <summary>
    /// Gets or sets the replacement. Refused when it matches the current one, so a forced rotation actually rotates.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    [Min(8)]
    [Required]
    [PasswordSecure]
    public required string NewPassword { get; set; }

    /// <summary>
    /// Gets or sets the repeat of the new password, refused when the two differ. Catches a typo before it becomes a
    /// credential nobody knows.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    [Min(8)]
    [Required]
    public required string ConfirmPassword { get; set; }
}
