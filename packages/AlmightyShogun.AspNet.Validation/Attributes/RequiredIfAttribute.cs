namespace AlmightyShogun.AspNet.Validation;

/// <summary>
/// Declares the required if validation rule for a request property.
/// </summary>
///
/// <param name="field">The request field that controls whether this field is required.</param>
/// <param name="values">The values that trigger required validation.</param>
///
/// <author>Almighty-Shogun</author>
/// <since>Unreleased</since>
[AttributeUsage(AttributeTargets.Property)]
public sealed class RequiredIfAttribute(string field, params object?[] values)
    : ValidationRuleAttribute(ConditionalTargetMode.Required, ConditionMode.If, field, values);
