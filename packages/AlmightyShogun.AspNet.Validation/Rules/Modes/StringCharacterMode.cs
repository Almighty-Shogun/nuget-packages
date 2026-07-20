namespace AlmightyShogun.AspNet.Validation;

/// <summary>
/// Defines the string character options used by validation rules.
/// </summary>
///
/// <author>Almighty-Shogun</author>
/// <since>Unreleased</since>
internal enum StringCharacterMode
{
    /// <summary>
    /// Requires alphabetic characters only.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    Alpha,

    /// <summary>
    /// Requires alphabetic or numeric characters only.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    AlphaNumeric,

    /// <summary>
    /// Requires alphabetic, numeric, dash, or underscore characters only.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    AlphaDash,

    /// <summary>
    /// Requires ASCII characters only.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    Ascii,

    /// <summary>
    /// Requires lowercase text.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    Lowercase,

    /// <summary>
    /// Requires uppercase text.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    Uppercase
}
