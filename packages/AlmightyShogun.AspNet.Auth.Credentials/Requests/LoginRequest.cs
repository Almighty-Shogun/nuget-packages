using AlmightyShogun.AspNet.RequestValidation;

namespace AlmightyShogun.AspNet.Auth.Credentials;

/// <summary>
/// A sign-in by username or email address. The two are accepted in one field because a user rarely remembers which they
/// registered with.
/// </summary>
///
/// <author>Almighty-Shogun</author>
/// <since>Unreleased</since>
public record LoginRequest
{
    /// <summary>
    /// Gets or sets the username or email address, matched against both. An unknown value fails exactly as a wrong password
    /// does, so neither reveals whether the account exists.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    [Required]
    public required string Identifier { get; set; }

    /// <summary>
    /// Gets or sets the submitted password, verified against the stored hash.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    [Required]
    public required string Password { get; set; }
}
