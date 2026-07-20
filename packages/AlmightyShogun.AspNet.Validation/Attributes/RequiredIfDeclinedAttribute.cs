namespace AlmightyShogun.AspNet.Validation;

/// <summary>
/// Declares the required if declined validation rule for a request property.
/// </summary>
///
/// <param name="field">The request field that triggers required validation when declined.</param>
///
/// <author>Almighty-Shogun</author>
/// <since>Unreleased</since>
[AttributeUsage(AttributeTargets.Property)]
public sealed class RequiredIfDeclinedAttribute(string field)
    : ValidationRuleAttribute(ConditionalStateTargetMode.Required, ConditionalStateMode.Declined, field);
