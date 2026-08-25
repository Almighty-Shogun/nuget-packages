namespace AlmightyShogun.AspNet.RequestValidation;

/// <summary>
/// Which quantity a size comparison measured, which is what lets one size rule report four different sentences.
/// </summary>
///
/// <author>Almighty-Shogun</author>
/// <since>Unreleased</since>
internal enum ValidationValueType
{
    /// <summary>
    /// The value's length in characters.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    String,

    /// <summary>
    /// The value itself.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    Numeric,

    /// <summary>
    /// The number of entries it holds.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    Array,

    /// <summary>
    /// Its size in kilobytes.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    File
}
