namespace AlmightyShogun.AspNet.RequestValidation;

/// <summary>
/// Which numeric shape the value must have, and for the decimal case how many places it may carry.
/// </summary>
///
/// <author>Almighty-Shogun</author>
/// <since>4.0.0</since>
internal enum NumberMode
{
    /// <summary>
    /// Any number, whole or fractional, including one written as text.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>4.0.0</since>
    Numeric,

    /// <summary>
    /// A whole number only, so a value with a fractional part fails even when that part is zero in text.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>4.0.0</since>
    Integer,

    /// <summary>
    /// Requires the configured number of decimal places.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>4.0.0</since>
    DecimalPlaces,

    /// <summary>
    /// Requires the value to be a multiple of the configured number.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>4.0.0</since>
    MultipleOf
}
