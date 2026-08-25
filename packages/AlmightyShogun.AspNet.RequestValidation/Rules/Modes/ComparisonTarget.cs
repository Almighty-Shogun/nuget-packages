namespace AlmightyShogun.AspNet.RequestValidation;

/// <summary>
/// Whether a comparison target names another field or is a literal value, which is otherwise indistinguishable since both arrive as text.
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
