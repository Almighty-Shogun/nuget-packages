namespace AlmightyShogun.AspNet.Validation;

/// <summary>
/// Declares the prohibited unless validation rule for a request property.
/// </summary>
///
/// <param name="field">The request field that controls whether prohibited validation is skipped.</param>
/// <param name="values">The values that skip prohibited validation.</param>
///
/// <author>Almighty-Shogun</author>
/// <since>Unreleased</since>
[AttributeUsage(AttributeTargets.Property)]
public sealed class ProhibitedUnlessAttribute(string field, params object?[] values)
    : ValidationRuleAttribute(ConditionalTargetMode.Prohibited, ConditionMode.Unless, field, values);
