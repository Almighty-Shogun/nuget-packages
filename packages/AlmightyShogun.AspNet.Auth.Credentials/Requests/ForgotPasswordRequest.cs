using AlmightyShogun.AspNet.RequestValidation;

namespace AlmightyShogun.AspNet.Auth.Credentials;

/// <summary>
/// The first half of a forgot-password flow. The service returns a token for a registered address and <c>null</c> for an
/// unregistered one, so answering the client identically either way is left to the controller above it.
/// </summary>
///
/// <author>Almighty-Shogun</author>
/// <since>Unreleased</since>
public sealed record ForgotPasswordRequest
{
    /// <summary>
    /// Gets or sets the address a reset link should go to, matched against the stored address under the column's own
    /// collation. One matching no
    /// account is not an error: the service returns <c>null</c> instead of throwing.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    [Email]
    [Required]
    public required string Email { get; set; }
}
