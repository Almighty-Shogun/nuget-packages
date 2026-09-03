using AlmightyShogun.AspNet.RequestValidation;

namespace AlmightyShogun.AspNet.Auth.Credentials;

/// <summary>
/// The first half of a forgot-password flow. The service answers the same way whether or not the address is registered,
/// so the response cannot be used to discover which addresses have accounts.
/// </summary>
///
/// <author>Almighty-Shogun</author>
/// <since>Unreleased</since>
public class ForgotPasswordRequest
{
    /// <summary>
    /// Gets or sets the address a reset link should go to. An address matching no account is not an error, so nothing
    /// in the response reveals whether it is registered.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    [Email]
    [Required]
    public required string Email { get; set; }
}
