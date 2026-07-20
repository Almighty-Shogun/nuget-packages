namespace AlmightyShogun.AspNet.Validation;

/// <summary>
/// Defines the number options used by validation rules.
/// </summary>
///
/// <author>Almighty-Shogun</author>
/// <since>Unreleased</since>
internal enum NumberMode
{
    /// <summary>
    /// Requires a numeric value.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    Numeric,

    /// <summary>
    /// Requires an integer value.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    Integer,

    /// <summary>
    /// Requires the configured number of decimal places.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    DecimalPlaces,

    /// <summary>
    /// Requires the value to be a multiple of the configured number.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    MultipleOf
}
