namespace AlmightyShogun.AspNet.RequestValidation;

/// <summary>
/// Which truthy state a controlling field must be in for a conditional rule to apply.
/// </summary>
///
/// <author>Almighty-Shogun</author>
/// <since>4.0.0</since>
internal enum ConditionalStateMode
{
    /// <summary>
    /// Uses the accepted state as the condition.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>4.0.0</since>
    Accepted,

    /// <summary>
    /// Uses the declined state as the condition.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>4.0.0</since>
    Declined
}
