namespace AlmightyShogun.AspNet.RequestValidation;

/// <summary>
/// Prohibits the field unless another field equals one of the provided values. Presence rules run before value rules, so a field this
/// rejects reports that rather than a later format or size failure.
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
