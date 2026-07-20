namespace AlmightyShogun.AspNet.Validation;

/// <summary>
/// Defines the conditional state target options used by validation rules.
/// </summary>
///
/// <author>Almighty-Shogun</author>
/// <since>Unreleased</since>
internal enum ConditionalStateTargetMode
{
    /// <summary>
    /// Requires the target value when the condition applies.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    Required,

    /// <summary>
    /// Prohibits the target value when the condition applies.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    Prohibited
}
