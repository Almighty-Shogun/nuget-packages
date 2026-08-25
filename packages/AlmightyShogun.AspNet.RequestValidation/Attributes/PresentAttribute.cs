namespace AlmightyShogun.AspNet.RequestValidation;

/// <summary>
/// Requires the field to exist in the request even when the value is allowed to be empty. Use it when the API must distinguish omitted
/// fields from explicitly provided empty values. Presence rules run before value rules, so a field this rejects reports that rather than a
/// later format or size failure.
/// </summary>
///
/// <author>Almighty-Shogun</author>
/// <since>Unreleased</since>
[AttributeUsage(AttributeTargets.Property)]
public sealed class PresentAttribute() : ValidationRuleAttribute(PresenceMode.Present);
