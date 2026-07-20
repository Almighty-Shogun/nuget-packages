namespace AlmightyShogun.AspNet.Validation;

/// <summary>
/// Declares the required unless validation rule for a request property.
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
