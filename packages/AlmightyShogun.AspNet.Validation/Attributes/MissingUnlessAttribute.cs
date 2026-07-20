namespace AlmightyShogun.AspNet.Validation;

/// <summary>
/// Declares the missing unless validation rule for a request property.
/// </summary>
///
/// <param name="field">The request field that controls whether missing validation is skipped.</param>
/// <param name="values">The values that skip missing validation.</param>
///
/// <author>Almighty-Shogun</author>
/// <since>Unreleased</since>
[AttributeUsage(AttributeTargets.Property)]
public sealed class MissingUnlessAttribute(string field, params object?[] values)
    : ValidationRuleAttribute(ConditionalTargetMode.Missing, ConditionMode.Unless, field, values);
