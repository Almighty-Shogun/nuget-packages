namespace AlmightyShogun.AspNet.RequestValidation;

/// <summary>
/// How this field must relate to another on the same request. The confirmation spelling differs from a plain match only in the field name
/// it defaults to.
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
