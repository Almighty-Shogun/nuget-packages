namespace AlmightyShogun.AspNet.Validation;

/// <summary>
/// Defines comparable validation value types.
/// </summary>
///
/// <author>Almighty-Shogun</author>
/// <since>Unreleased</since>
internal enum ValidationValueType
{
    /// <summary>
    /// Treats the value as text.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    String,

    /// <summary>
    /// Treats the value as a number.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    Numeric,

    /// <summary>
    /// Treats the value as a collection.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    Array,

    /// <summary>
    /// Treats the value as an uploaded file.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    File
}
