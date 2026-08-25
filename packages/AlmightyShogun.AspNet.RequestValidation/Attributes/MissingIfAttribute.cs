namespace AlmightyShogun.AspNet.RequestValidation;

/// <summary>
/// Requires the field to be missing when another field equals one of the provided values. Presence rules run before value rules, so a field
/// this rejects reports that rather than a later format or size failure.
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
