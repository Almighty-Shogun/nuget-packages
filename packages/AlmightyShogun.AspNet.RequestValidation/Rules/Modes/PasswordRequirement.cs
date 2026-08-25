namespace AlmightyShogun.AspNet.RequestValidation;

/// <summary>
/// Which character class a password must include. Each is a separate requirement so an application composes the policy it wants rather than
/// accepting one fixed definition of strong.
/// </summary>
///
/// <author>Almighty-Shogun</author>
/// <since>Unreleased</since>
internal enum PasswordRequirement
{
    /// <summary>
    /// Requires at least one letter.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    Letters,

    /// <summary>
    /// Requires both uppercase and lowercase letters.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    Mixed,

    /// <summary>
    /// Requires at least one number.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    Numbers,

    /// <summary>
    /// Requires at least one symbol.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    Symbols,

    /// <summary>
    /// Requires all built-in password strength requirements.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    Secure
}
