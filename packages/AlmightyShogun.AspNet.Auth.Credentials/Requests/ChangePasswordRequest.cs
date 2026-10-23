using AlmightyShogun.AspNet.RequestValidation;

namespace AlmightyShogun.AspNet.Auth.Credentials;

/// <summary>
/// Represents a password change request for an authenticated user.
/// </summary>
///
/// <remarks>
/// Password strength is validated by request attributes, not by the service.
/// </remarks>
/// 
/// <author>Almighty-Shogun</author>
/// <since>4.0.0</since>
public sealed class ChangePasswordRequest
{
    /// <summary>
    /// The user's current password.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>4.0.0</since>
    [Required]
    public string CurrentPassword { get; set; } = string.Empty;

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
