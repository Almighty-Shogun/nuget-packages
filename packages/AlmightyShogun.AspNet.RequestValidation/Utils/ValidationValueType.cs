namespace AlmightyShogun.AspNet.RequestValidation;

/// <summary>
/// Which quantity a size comparison measured, which is what lets one size rule report four different sentences.
/// </summary>
///
/// <author>Almighty-Shogun</author>
/// <since>4.0.0</since>
internal enum ValidationValueType
{
    /// <summary>
    /// The value's length in characters.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>4.0.0</since>
    String,

    /// <summary>
    /// The value itself.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>4.0.0</since>
    Numeric,

    /// <summary>
    /// The number of entries it holds.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>4.0.0</since>
    Array,

    /// <summary>
    /// Its size in kilobytes.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>4.0.0</since>
    File
}
