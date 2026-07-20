namespace AlmightyShogun.AspNet.Validation;

/// <summary>
/// Declares the missing if validation rule for a request property.
/// </summary>
///
/// <param name="field">The request field that controls whether this field must be missing.</param>
/// <param name="values">The values that trigger missing validation.</param>
///
/// <author>Almighty-Shogun</author>
/// <since>Unreleased</since>
[AttributeUsage(AttributeTargets.Property)]
public sealed class MissingIfAttribute(string field, params object?[] values)
    : ValidationRuleAttribute(ConditionalTargetMode.Missing, ConditionMode.If, field, values);
