namespace AlmightyShogun.AspNet.Validation;

/// <summary>
/// Declares the prohibited if declined validation rule for a request property.
/// </summary>
///
/// <param name="field">The request field that triggers prohibited validation when declined.</param>
///
/// <author>Almighty-Shogun</author>
/// <since>Unreleased</since>
[AttributeUsage(AttributeTargets.Property)]
public sealed class ProhibitedIfDeclinedAttribute(string field)
    : ValidationRuleAttribute(ConditionalStateTargetMode.Prohibited, ConditionalStateMode.Declined, field);
