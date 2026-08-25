namespace AlmightyShogun.AspNet.RequestValidation;

/// <summary>
/// Requires the value to be after a literal date or another request field. An absent or empty value passes, so pair it with
/// <see cref="RequiredAttribute"/> when the field is mandatory.
/// </summary>
///
/// <param name="target">The comparison date value or request field name.</param>
/// <param name="targetType">Whether the target is a literal value or a request field.</param>
///
/// <author>Almighty-Shogun</author>
/// <since>Unreleased</since>
[AttributeUsage(AttributeTargets.Property)]
public sealed class AfterAttribute(string target, ComparisonTarget targetType = ComparisonTarget.Value)
    : ValidationRuleAttribute(DateMode.After, target, targetType is ComparisonTarget.Field);
