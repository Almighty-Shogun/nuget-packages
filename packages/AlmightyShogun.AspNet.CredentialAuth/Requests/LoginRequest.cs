using AlmightyShogun.AspNet.Validation;

namespace AlmightyShogun.AspNet.CredentialAuth;

/// <summary>
/// Represents a login request using a username or email address and password.
/// </summary>
///
/// <author>Almighty-Shogun</author>
/// <since>Unreleased</since>
public record LoginRequest
{
    /// <summary>
    /// Gets or sets the username or email address.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    [Required]
    [LoginIdentifierExists]
    public required string Identifier { get; set; }

    /// <summary>
    /// Gets or sets the plain-text password.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    [Required]
    [CurrentPassword]
    public required string Password { get; set; }
}
