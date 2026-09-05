namespace AlmightyShogun.AspNet.RequestValidation;

/// <summary>
/// What a state-conditional rule does to its own field once the condition holds: demand a value or forbid one.
/// </summary>
///
/// <author>Almighty-Shogun</author>
/// <since>4.0.0</since>
internal enum ConditionalStateTargetMode
{
    /// <summary>
    /// Requires the target value when the condition applies.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>4.0.0</since>
    Required,

    /// <summary>
    /// Prohibits the target value when the condition applies.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>4.0.0</since>
    Prohibited
}
