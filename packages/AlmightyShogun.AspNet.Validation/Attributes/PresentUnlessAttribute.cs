namespace AlmightyShogun.AspNet.Validation;

/// <summary>
/// Declares the present unless validation rule for a request property.
/// </summary>
///
/// <param name="field">The request field that controls whether present validation is skipped.</param>
/// <param name="values">The values that skip present validation.</param>
///
/// <author>Almighty-Shogun</author>
/// <since>Unreleased</since>
[AttributeUsage(AttributeTargets.Property)]
public sealed class PresentUnlessAttribute(string field, params object?[] values)
    : ValidationRuleAttribute(ConditionalTargetMode.Present, ConditionMode.Unless, field, values);
