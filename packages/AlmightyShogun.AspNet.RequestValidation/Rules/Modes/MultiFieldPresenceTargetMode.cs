namespace AlmightyShogun.AspNet.RequestValidation;

/// <summary>
/// What a multi-field rule does to its own field once the trigger holds: demand a value, demand presence, or forbid it.
/// </summary>
///
/// <author>Almighty-Shogun</author>
/// <since>Unreleased</since>
internal enum MultiFieldPresenceTargetMode
{
    /// <summary>
    /// Requires the target value when related field rules apply.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    Required,

    /// <summary>
    /// Requires the target field to be present when related field rules apply.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    Present,

    /// <summary>
    /// Requires the target field to be missing when related field rules apply.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    Missing,

    /// <summary>
    /// Prohibits the target value when related fields are present.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    Prohibits
}
