namespace AlmightyShogun.AspNet.RequestValidation;

/// <summary>
/// Which end of a size comparison applies. The measured quantity is decided by the bound type rather than by this, so the same mode
/// constrains a number's value, a string's length, a collection's count, or a file's size.
/// </summary>
///
/// <author>Almighty-Shogun</author>
/// <since>Unreleased</since>
internal enum ComparableSizeMode
{
    /// <summary>
    /// Requires the value to be at least the configured minimum.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    Min,

    /// <summary>
    /// Requires the value to be no greater than the configured maximum.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    Max,

    /// <summary>
    /// Requires the value to be between the configured bounds.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    Between,

    /// <summary>
    /// Requires the value to have the exact configured size.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    Size,

    /// <summary>
    /// Requires the value to be greater than the configured value.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    GreaterThan,

    /// <summary>
    /// Requires the value to be greater than or equal to the configured value.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    GreaterThanOrEqual,

    /// <summary>
    /// Requires the value to be less than the configured value.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    LessThan,

    /// <summary>
    /// Requires the value to be less than or equal to the configured value.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    LessThanOrEqual
}
