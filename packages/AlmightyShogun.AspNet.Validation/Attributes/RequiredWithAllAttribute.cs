namespace AlmightyShogun.AspNet.Validation;

/// <summary>
/// Declares the required with all validation rule for a request property.
/// </summary>
///
/// <param name="fields">The request fields that trigger required validation when all are present.</param>
///
/// <author>Almighty-Shogun</author>
/// <since>Unreleased</since>
[AttributeUsage(AttributeTargets.Property)]
public sealed class RequiredWithAllAttribute(params string[] fields)
    : ValidationRuleAttribute(MultiFieldPresenceTargetMode.Required, MultiFieldPresenceTriggerMode.WithAll, fields);
