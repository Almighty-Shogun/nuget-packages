namespace AlmightyShogun.AspNet.RequestValidation;

/// <summary>
/// Requires the field unless another field equals one of the provided values. Presence rules run before value rules, so a field this
/// rejects reports that rather than a later format or size failure.
/// </summary>
///
/// <param name="field">The request field that controls whether required validation is skipped.</param>
/// <param name="values">The values that skip required validation.</param>
///
/// <author>Almighty-Shogun</author>
/// <since>Unreleased</since>
[AttributeUsage(AttributeTargets.Property)]
public sealed class RequiredUnlessAttribute(string field, params object?[] values)
    : ValidationRuleAttribute(ConditionalTargetMode.Required, ConditionMode.Unless, field, values);
