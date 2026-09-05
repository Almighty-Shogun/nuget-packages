namespace AlmightyShogun.AspNet.RequestValidation;

/// <summary>
/// Which date check applies: that the value parses at all, that it matches an exact format, or how it orders against a target.
/// </summary>
///
/// <author>Almighty-Shogun</author>
/// <since>4.0.0</since>
internal enum DateMode
{
    /// <summary>
    /// Requires the value to be a valid date.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>4.0.0</since>
    ValidDate,

    /// <summary>
    /// Requires the value to match the configured date format.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>4.0.0</since>
    ExactFormat,

    /// <summary>
    /// Requires the date to be after the target date.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>4.0.0</since>
    After,

    /// <summary>
    /// Requires the date to be after or equal to the target date.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>4.0.0</since>
    AfterOrEqual,

    /// <summary>
    /// Requires the date to be before the target date.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>4.0.0</since>
    Before,

    /// <summary>
    /// Requires the date to be before or equal to the target date.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>4.0.0</since>
    BeforeOrEqual,

    /// <summary>
    /// Requires the date to equal the target date.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>4.0.0</since>
    Equals
}
