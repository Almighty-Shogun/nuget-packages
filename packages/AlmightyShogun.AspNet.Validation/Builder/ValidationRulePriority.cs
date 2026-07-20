namespace AlmightyShogun.AspNet.Validation;

/// <summary>
/// Defines validation rule execution priority.
/// </summary>
///
/// <author>Almighty-Shogun</author>
/// <since>Unreleased</since>
internal enum ValidationRulePriority
{
    /// <summary>
    /// Runs the rule before normal validation rules.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    Required,

    /// <summary>
    /// Runs the rule with normal validation priority.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    Normal
}
