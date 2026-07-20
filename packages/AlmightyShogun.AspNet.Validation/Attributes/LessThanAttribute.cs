namespace AlmightyShogun.AspNet.Validation;

/// <summary>
/// Declares the less than validation rule for a request property.
/// </summary>
///
/// <param name="value">The exclusive maximum value.</param>
///
/// <author>Almighty-Shogun</author>
/// <since>Unreleased</since>
[AttributeUsage(AttributeTargets.Property)]
public sealed class LessThanAttribute(double value) : ValidationRuleAttribute(ComparableSizeMode.LessThan, value);
