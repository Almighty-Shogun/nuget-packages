namespace AlmightyShogun.AspNet.Validation;

/// <summary>
/// Defines the field comparison options used by validation rules.
/// </summary>
///
/// <author>Almighty-Shogun</author>
/// <since>Unreleased</since>
internal enum FieldComparisonMode
{
    /// <summary>
    /// Requires the target value to match another field value.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    Same,

    /// <summary>
    /// Requires the target value to differ from another field value.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    Different,

    /// <summary>
    /// Requires the target value to match the conventional confirmation field.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    Confirmed
}
