namespace AlmightyShogun.AspNet.Validation;

/// <summary>
/// Declares the required without validation rule for a request property.
/// </summary>
///
/// <param name="fields">The request fields that trigger required validation when any are missing.</param>
///
/// <author>Almighty-Shogun</author>
/// <since>Unreleased</since>
[AttributeUsage(AttributeTargets.Property)]
public sealed class RequiredWithoutAttribute(params string[] fields)
    : ValidationRuleAttribute(MultiFieldPresenceTargetMode.Required, MultiFieldPresenceTriggerMode.WithoutAny, fields);
