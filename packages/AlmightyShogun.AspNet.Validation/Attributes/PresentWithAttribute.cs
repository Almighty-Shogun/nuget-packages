namespace AlmightyShogun.AspNet.Validation;

/// <summary>
/// Declares the present with validation rule for a request property.
/// </summary>
///
/// <param name="fields">The request fields that trigger present validation when any are present.</param>
///
/// <author>Almighty-Shogun</author>
/// <since>Unreleased</since>
[AttributeUsage(AttributeTargets.Property)]
public sealed class PresentWithAttribute(params string[] fields)
    : ValidationRuleAttribute(MultiFieldPresenceTargetMode.Present, MultiFieldPresenceTriggerMode.WithAny, fields);
