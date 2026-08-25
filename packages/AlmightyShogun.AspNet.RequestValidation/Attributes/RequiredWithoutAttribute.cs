namespace AlmightyShogun.AspNet.RequestValidation;

/// <summary>
/// Requires the field when any listed field is missing. Presence rules run before value rules, so a field this rejects reports that rather
/// than a later format or size failure.
/// </summary>
///
/// <param name="fields">The request fields that trigger required validation when any are missing.</param>
///
/// <author>Almighty-Shogun</author>
/// <since>Unreleased</since>
[AttributeUsage(AttributeTargets.Property)]
public sealed class RequiredWithoutAttribute(params string[] fields)
    : ValidationRuleAttribute(MultiFieldPresenceTargetMode.Required, MultiFieldPresenceTriggerMode.WithoutAny, fields);
