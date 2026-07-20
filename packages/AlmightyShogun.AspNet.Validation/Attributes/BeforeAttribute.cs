namespace AlmightyShogun.AspNet.Validation;

/// <summary>
/// Declares the before validation rule for a request property.
/// </summary>
///
/// <param name="target">The comparison date value or request field name.</param>
/// <param name="targetType">Whether the target is a literal value or a request field.</param>
///
/// <author>Almighty-Shogun</author>
/// <since>Unreleased</since>
[AttributeUsage(AttributeTargets.Property)]
public sealed class BeforeAttribute(string target, ComparisonTarget targetType = ComparisonTarget.Value)
    : ValidationRuleAttribute(DateMode.Before, target, targetType == ComparisonTarget.Field);
