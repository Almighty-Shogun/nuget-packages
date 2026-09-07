using AlmightyShogun.AspNet.RequestValidation;

namespace AlmightyShogun.AspNet.Auth.Credentials;

/// <summary>
/// The second half of a sign-in that stopped at the second factor, presenting the challenge alongside the code it is owed
/// against. The challenge is spent on success, so the same one cannot open a second session.
/// </summary>
///
/// <author>Almighty-Shogun</author>
/// <since>4.1.0</since>
public sealed record CompleteTwoFactorLoginRequest
{
    /// <summary>
    /// The challenge as the sign-in returned it, in the form it was handed back rather than a decoded or trimmed one.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>4.1.0</since>
    [Required]
    public string Challenge { get; set; } = string.Empty;

    /// <summary>
    /// The submitted value, tried as a TOTP code and then as a recovery code, so nothing here says which it is.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>4.1.0</since>
    [Required]
    public string Code { get; set; } = string.Empty;
}
