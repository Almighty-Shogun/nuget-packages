namespace AlmightyShogun.AspNet.Validation;

/// <summary>
/// Declares the accepted if validation rule for a request property.
/// </summary>
///
/// <param name="field">The request field that controls whether this field must be accepted.</param>
/// <param name="values">The values that trigger accepted validation.</param>
///
/// <author>Almighty-Shogun</author>
/// <since>Unreleased</since>
[AttributeUsage(AttributeTargets.Property)]
public sealed class AcceptedIfAttribute(string field, params object?[] values)
    : ValidationRuleAttribute(ConditionalTargetMode.Accepted, ConditionMode.If, field, values);
