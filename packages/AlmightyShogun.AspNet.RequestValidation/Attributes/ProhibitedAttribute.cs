namespace AlmightyShogun.AspNet.RequestValidation;

/// <summary>
/// Rejects the field when the client sends it with a non-empty value. Empty or missing values are allowed. Presence rules run before value
/// rules, so a field this rejects reports that rather than a later format or size failure.
/// </summary>
///
/// <author>Almighty-Shogun</author>
/// <since>Unreleased</since>
[AttributeUsage(AttributeTargets.Property)]
public sealed class ProhibitedAttribute() : ValidationRuleAttribute(PresenceMode.Prohibited);
