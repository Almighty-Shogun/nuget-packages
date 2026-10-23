using AlmightyShogun.AspNet.RequestValidation;

namespace AlmightyShogun.AspNet.Auth.Credentials;

/// <summary>
/// Represents a request to initiate password recovery.
/// </summary>
///
/// <remarks>
/// Controllers should return identical responses for registered and unregistered email addresses
/// to avoid account enumeration.
/// </remarks>
/// 
/// <author>Almighty-Shogun</author>
/// <since>4.0.0</since>
public sealed record ForgotPasswordRequest
{
    /// <summary>
    /// The email address associated with the account.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>4.0.0</since>
    [Email]
    [Required]
    public string Email { get; set; } = string.Empty;
}
