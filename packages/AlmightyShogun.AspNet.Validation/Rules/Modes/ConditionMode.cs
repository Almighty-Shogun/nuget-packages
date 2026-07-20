namespace AlmightyShogun.AspNet.Validation;

/// <summary>
/// Defines the condition options used by validation rules.
/// </summary>
///
/// <author>Almighty-Shogun</author>
/// <since>Unreleased</since>
internal enum ConditionMode
{
    /// <summary>
    /// Runs the rule when the condition matches.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    If,

    /// <summary>
    /// Runs the rule when the condition does not match.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    Unless
}
