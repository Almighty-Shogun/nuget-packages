namespace AlmightyShogun.AspNet.Validation;

/// <summary>
/// Declares the missing with all validation rule for a request property.
/// </summary>
///
/// <param name="fields">The request fields that trigger missing validation when all are present.</param>
///
/// <author>Almighty-Shogun</author>
/// <since>Unreleased</since>
[AttributeUsage(AttributeTargets.Property)]
public sealed class MissingWithAllAttribute(params string[] fields)
    : ValidationRuleAttribute(MultiFieldPresenceTargetMode.Missing, MultiFieldPresenceTriggerMode.WithAll, fields);
