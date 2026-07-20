namespace AlmightyShogun.AspNet.Validation;

/// <summary>
/// Declares the min validation rule for a request property.
/// </summary>
///
/// <param name="value">The inclusive minimum value.</param>
///
/// <author>Almighty-Shogun</author>
/// <since>Unreleased</since>
[AttributeUsage(AttributeTargets.Property)]
public sealed class MinAttribute(double value) : ValidationRuleAttribute(ComparableSizeMode.Min, value);
