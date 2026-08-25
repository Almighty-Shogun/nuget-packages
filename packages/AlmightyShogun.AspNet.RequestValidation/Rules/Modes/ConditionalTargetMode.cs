namespace AlmightyShogun.AspNet.RequestValidation;

/// <summary>
/// What a value-conditional rule does to its own field once the condition holds. Requiring a value and requiring mere presence are
/// separate, because a field posted empty is present without holding anything.
/// </summary>
///
/// <author>Almighty-Shogun</author>
/// <since>Unreleased</since>
internal enum ConditionalTargetMode
{
    /// <summary>
    /// Requires the target value when the condition applies.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    Required,

    /// <summary>
    /// Requires the target field to be present when the condition applies.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    Present,

    /// <summary>
    /// Requires the target field to be missing when the condition applies.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    Missing,

    /// <summary>
    /// Prohibits the target value when the condition applies.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    Prohibited,

    /// <summary>
    /// Requires the target value to be accepted when the condition applies.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    Accepted,

    /// <summary>
    /// Requires the target value to be declined when the condition applies.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    Declined
}
