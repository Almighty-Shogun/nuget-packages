namespace AlmightyShogun.AspNet.Validation;

/// <summary>
/// Declares the greater than validation rule for a request property.
/// </summary>
///
/// <param name="value">The exclusive minimum value.</param>
///
/// <author>Almighty-Shogun</author>
/// <since>Unreleased</since>
[AttributeUsage(AttributeTargets.Property)]
public sealed class GreaterThanAttribute(double value) : ValidationRuleAttribute(ComparableSizeMode.GreaterThan, value);
