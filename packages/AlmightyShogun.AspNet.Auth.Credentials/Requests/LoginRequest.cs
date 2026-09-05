using AlmightyShogun.AspNet.RequestValidation;

namespace AlmightyShogun.AspNet.Auth.Credentials;

/// <summary>
/// A sign-in by username or email address. The two are accepted in one field because a user rarely remembers which they
/// registered with.
/// </summary>
///
/// <author>Almighty-Shogun</author>
/// <since>Unreleased</since>
public sealed record LoginRequest
{
    /// <summary>
    /// Gets or sets the username or email address, matched against both. An unknown value is refused with the same exception as a wrong
    /// password and costs a decoy verification, so the response does not reveal whether the account exists. With lockout
    /// enabled a known identifier also runs the lockout statements an unknown one never reaches.
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
