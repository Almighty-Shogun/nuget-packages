using AlmightyShogun.AspNet.RequestValidation;

namespace AlmightyShogun.AspNet.Auth.Credentials;

/// <summary>
/// Represents a request to complete email verification using a token.
/// </summary>
///
/// <author>Almighty-Shogun</author>
/// <since>4.1.0</since>
public sealed record CompleteEmailVerificationRequest
{
    /// <summary>
    /// The email verification token.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>4.1.0</since>
    [Required]
    public string Token { get; set; } = string.Empty;
}
