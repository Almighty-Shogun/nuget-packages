namespace AlmightyShogun.AspNet.RequestValidation;

/// <summary>
/// Requires the field when another field contains an accepted value such as <c>true</c> , <c>yes</c> , <c>on</c> , or <c>1</c> . Presence
/// rules run before value rules, so a field this rejects reports that rather than a later format or size failure.
/// </summary>
///
/// <param name="field">The request field that triggers required validation when accepted.</param>
///
/// <author>Almighty-Shogun</author>
/// <since>Unreleased</since>
[AttributeUsage(AttributeTargets.Property)]
public sealed class RequiredIfAcceptedAttribute(string field)
    : ValidationRuleAttribute(ConditionalStateTargetMode.Required, ConditionalStateMode.Accepted, field);
