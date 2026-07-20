namespace AlmightyShogun.AspNet.Validation;

/// <summary>
/// Declares the declined if validation rule for a request property.
/// </summary>
///
/// <param name="field">The request field that controls whether this field must be declined.</param>
/// <param name="values">The values that trigger declined validation.</param>
///
/// <author>Almighty-Shogun</author>
/// <since>Unreleased</since>
[AttributeUsage(AttributeTargets.Property)]
public sealed class DeclinedIfAttribute(string field, params object?[] values)
    : ValidationRuleAttribute(ConditionalTargetMode.Declined, ConditionMode.If, field, values);
