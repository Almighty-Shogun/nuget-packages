namespace AlmightyShogun.AspNet.Validation;

/// <summary>
/// Defines the password requirement options used by validation rules.
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
