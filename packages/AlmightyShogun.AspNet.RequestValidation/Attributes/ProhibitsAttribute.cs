namespace AlmightyShogun.AspNet.RequestValidation;

/// <summary>
/// Makes this field prohibit the listed fields from being present with a value. Use it for mutually exclusive request options. Presence
/// rules run before value rules, so a field this rejects reports that rather than a later format or size failure.
/// </summary>
///
/// <param name="fields">The request fields that are prohibited when this field is present.</param>
///
/// <author>Almighty-Shogun</author>
/// <since>Unreleased</since>
[AttributeUsage(AttributeTargets.Property)]
public sealed class ProhibitsAttribute(params string[] fields)
    : ValidationRuleAttribute(MultiFieldPresenceTargetMode.Prohibits, MultiFieldPresenceTriggerMode.Prohibits, fields);
