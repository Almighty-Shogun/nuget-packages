namespace AlmightyShogun.AspNet.Validation;

/// <summary>
/// Declares the prohibits validation rule for a request property.
/// </summary>
///
/// <param name="fields">The request fields that are prohibited when this field is present.</param>
///
/// <author>Almighty-Shogun</author>
/// <since>Unreleased</since>
[AttributeUsage(AttributeTargets.Property)]
public sealed class ProhibitsAttribute(params string[] fields)
    : ValidationRuleAttribute(MultiFieldPresenceTargetMode.Prohibits, MultiFieldPresenceTriggerMode.Prohibits, fields);
