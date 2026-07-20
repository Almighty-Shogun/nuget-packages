namespace AlmightyShogun.AspNet.Validation;

/// <summary>
/// Declares the max validation rule for a request property.
/// </summary>
///
/// <param name="value">The inclusive maximum value.</param>
///
/// <author>Almighty-Shogun</author>
/// <since>Unreleased</since>
[AttributeUsage(AttributeTargets.Property)]
public sealed class MaxAttribute(double value) : ValidationRuleAttribute(ComparableSizeMode.Max, value);
