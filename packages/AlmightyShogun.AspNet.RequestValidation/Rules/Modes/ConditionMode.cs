namespace AlmightyShogun.AspNet.RequestValidation;

/// <summary>
/// Whether a conditional rule fires on a match or on the absence of one, which is what separates the if and unless spellings.
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
