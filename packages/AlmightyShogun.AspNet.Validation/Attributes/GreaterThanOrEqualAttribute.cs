namespace AlmightyShogun.AspNet.Validation;

/// <summary>
/// Declares the greater than or equal validation rule for a request property.
/// </summary>
///
/// <param name="value">The inclusive minimum value.</param>
///
/// <author>Almighty-Shogun</author>
/// <since>Unreleased</since>
[AttributeUsage(AttributeTargets.Property)]
public sealed class GreaterThanOrEqualAttribute(double value)
    : ValidationRuleAttribute(ComparableSizeMode.GreaterThanOrEqual, value);
