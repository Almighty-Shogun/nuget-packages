using AlmightyShogun.AspNet.RequestValidation;

namespace AlmightyShogun.AspNet.Auth.Credentials;

/// <summary>
/// The second half of either verification flow, presenting the token from the email. The same shape serves both, because
/// the method being called is what decides which purpose the token has to carry. The token is spent on success, so a
/// second submission of the same one is refused.
/// </summary>
///
/// <author>Almighty-Shogun</author>
/// <since>4.1.0</since>
public sealed record CompleteEmailVerificationRequest
{
    /// <summary>
    /// The token from the verification email, in the form it was sent rather than a decoded or trimmed one.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>4.1.0</since>
    [Required]
    public string Token { get; set; } = string.Empty;
}
