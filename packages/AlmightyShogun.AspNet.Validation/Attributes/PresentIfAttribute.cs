namespace AlmightyShogun.AspNet.Validation;

/// <summary>
/// Declares the present if validation rule for a request property.
/// </summary>
///
/// <param name="field">The request field that controls whether this field must be present.</param>
/// <param name="values">The values that trigger present validation.</param>
///
/// <author>Almighty-Shogun</author>
/// <since>Unreleased</since>
[AttributeUsage(AttributeTargets.Property)]
public sealed class PresentIfAttribute(string field, params object?[] values)
    : ValidationRuleAttribute(ConditionalTargetMode.Present, ConditionMode.If, field, values);
