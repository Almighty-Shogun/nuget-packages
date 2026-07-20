namespace AlmightyShogun.AspNet.Validation;

/// <summary>
/// Declares the less than or equal validation rule for a request property.
/// </summary>
///
/// <param name="value">The inclusive maximum value.</param>
///
/// <author>Almighty-Shogun</author>
/// <since>Unreleased</since>
[AttributeUsage(AttributeTargets.Property)]
public sealed class LessThanOrEqualAttribute(double value) : ValidationRuleAttribute(ComparableSizeMode.LessThanOrEqual, value);
