namespace AlmightyShogun.AspNet.Validation;

/// <summary>
/// Declares the prohibited if accepted validation rule for a request property.
/// </summary>
///
/// <param name="field">The request field that triggers prohibited validation when accepted.</param>
///
/// <author>Almighty-Shogun</author>
/// <since>Unreleased</since>
[AttributeUsage(AttributeTargets.Property)]
public sealed class ProhibitedIfAcceptedAttribute(string field)
    : ValidationRuleAttribute(ConditionalStateTargetMode.Prohibited, ConditionalStateMode.Accepted, field);
