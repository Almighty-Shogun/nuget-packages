namespace AlmightyShogun.AspNet.RequestValidation;

/// <summary>
/// Which characters the text may contain, or the case it must already be written in.
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
    /// The text must already be lowercase. It is not lowercased for you, so a mixed-case value fails rather than being corrected.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    Lowercase,

    /// <summary>
    /// The text must already be uppercase, on the same terms as its lowercase counterpart.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    Uppercase
}
