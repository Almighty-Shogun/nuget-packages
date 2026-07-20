namespace AlmightyShogun.AspNet.Validation;

/// <summary>
/// Defines how validation rules resolve a comparison target.
/// </summary>
///
/// <author>Almighty-Shogun</author>
/// <since>Unreleased</since>
public enum ComparisonTarget
{
    /// <summary>
    /// Uses the supplied target value as a literal comparison value.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    Value,

    /// <summary>
    /// Uses the supplied target value as the name of another request field.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    Field
}
