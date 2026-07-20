namespace AlmightyShogun.AspNet.Validation;

/// <summary>
/// Declares the required if accepted validation rule for a request property.
/// </summary>
///
/// <param name="field">The request field that triggers required validation when accepted.</param>
///
/// <author>Almighty-Shogun</author>
/// <since>Unreleased</since>
[AttributeUsage(AttributeTargets.Property)]
public sealed class RequiredIfAcceptedAttribute(string field)
    : ValidationRuleAttribute(ConditionalStateTargetMode.Required, ConditionalStateMode.Accepted, field);
