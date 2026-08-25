namespace AlmightyShogun.AspNet.RequestValidation;

/// <summary>
/// Requires the field to contain a value when the client sends it. Missing values are allowed, but present empty values fail validation.
/// Presence rules run before value rules, so a field this rejects reports that rather than a later format or size failure.
/// </summary>
///
/// <author>Almighty-Shogun</author>
/// <since>Unreleased</since>
[AttributeUsage(AttributeTargets.Property)]
public sealed class FilledAttribute() : ValidationRuleAttribute(PresenceMode.Filled);
