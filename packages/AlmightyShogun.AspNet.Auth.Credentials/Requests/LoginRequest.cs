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
    /// The username or email address, matched against both columns rather than against one chosen by shape.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    [Required]
    public required string Identifier { get; set; }

    /// <summary>
    /// The submitted password, verified against the stored hash.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    [Required]
    public required string Password { get; set; }
}
