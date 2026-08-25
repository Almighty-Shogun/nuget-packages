namespace AlmightyShogun.AspNet.RequestValidation;

/// <summary>
/// Which truthy state a controlling field must be in for a conditional rule to apply.
/// </summary>
///
/// <author>Almighty-Shogun</author>
/// <since>Unreleased</since>
internal enum ConditionalStateMode
{
    /// <summary>
    /// Uses the accepted state as the condition.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    Accepted,

    /// <summary>
    /// Uses the declined state as the condition.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    Declined
}
