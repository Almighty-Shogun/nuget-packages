namespace AlmightyShogun.AspNet.RequestValidation;

/// <summary>
/// Whether a digit count must be exact, at least, at most, or within a range.
/// </summary>
///
/// <author>Almighty-Shogun</author>
/// <since>4.0.0</since>
internal enum DigitMode
{
    /// <summary>
    /// Requires the exact configured digit count.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>4.0.0</since>
    Exact,

    /// <summary>
    /// Requires the digit count to be between the configured bounds.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>4.0.0</since>
    Between,

    /// <summary>
    /// Requires at least the configured digit count.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>4.0.0</since>
    Min,

    /// <summary>
    /// Requires no more than the configured digit count.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>4.0.0</since>
    Max
}
