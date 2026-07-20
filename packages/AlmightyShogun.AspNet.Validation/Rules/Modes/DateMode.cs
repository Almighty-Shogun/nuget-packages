namespace AlmightyShogun.AspNet.Validation;

/// <summary>
/// Defines the date options used by validation rules.
/// </summary>
///
/// <author>Almighty-Shogun</author>
/// <since>Unreleased</since>
internal enum DateMode
{
    /// <summary>
    /// Requires the value to be a valid date.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    ValidDate,

    /// <summary>
    /// Requires the value to match the configured date format.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    ExactFormat,

    /// <summary>
    /// Requires the date to be after the target date.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    After,

    /// <summary>
    /// Requires the date to be after or equal to the target date.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    AfterOrEqual,

    /// <summary>
    /// Requires the date to be before the target date.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    Before,

    /// <summary>
    /// Requires the date to be before or equal to the target date.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    BeforeOrEqual,

    /// <summary>
    /// Requires the date to equal the target date.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    Equals
}
