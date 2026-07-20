namespace AlmightyShogun.AspNet.Validation;

/// <summary>
/// Declares the missing with validation rule for a request property.
/// </summary>
///
/// <param name="fields">The request fields that trigger missing validation when any are present.</param>
///
/// <author>Almighty-Shogun</author>
/// <since>Unreleased</since>
[AttributeUsage(AttributeTargets.Property)]
public sealed class MissingWithAttribute(params string[] fields)
    : ValidationRuleAttribute(MultiFieldPresenceTargetMode.Missing, MultiFieldPresenceTriggerMode.WithAny, fields);
