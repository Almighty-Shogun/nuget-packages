namespace AlmightyShogun.AspNet.Validation;

/// <summary>
/// Defines the conditional state options used by validation rules.
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
