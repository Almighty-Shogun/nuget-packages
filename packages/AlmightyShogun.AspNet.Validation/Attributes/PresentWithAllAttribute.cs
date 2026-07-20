namespace AlmightyShogun.AspNet.Validation;

/// <summary>
/// Declares the present with all validation rule for a request property.
/// </summary>
///
/// <param name="fields">The request fields that trigger present validation when all are present.</param>
///
/// <author>Almighty-Shogun</author>
/// <since>Unreleased</since>
[AttributeUsage(AttributeTargets.Property)]
public sealed class PresentWithAllAttribute(params string[] fields)
    : ValidationRuleAttribute(MultiFieldPresenceTargetMode.Present, MultiFieldPresenceTriggerMode.WithAll, fields);
