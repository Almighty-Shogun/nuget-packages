namespace AlmightyShogun.AspNet.Validation;

/// <summary>
/// Declares the prohibited if validation rule for a request property.
/// </summary>
///
/// <param name="field">The request field that controls whether this field is prohibited.</param>
/// <param name="values">The values that trigger prohibited validation.</param>
///
/// <author>Almighty-Shogun</author>
/// <since>Unreleased</since>
[AttributeUsage(AttributeTargets.Property)]
public sealed class ProhibitedIfAttribute(string field, params object?[] values)
    : ValidationRuleAttribute(ConditionalTargetMode.Prohibited, ConditionMode.If, field, values);
