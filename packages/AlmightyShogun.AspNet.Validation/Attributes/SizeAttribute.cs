namespace AlmightyShogun.AspNet.Validation;

/// <summary>
/// Declares the size validation rule for a request property.
/// </summary>
///
/// <param name="value">The exact size value.</param>
///
/// <author>Almighty-Shogun</author>
/// <since>Unreleased</since>
[AttributeUsage(AttributeTargets.Property)]
public sealed class SizeAttribute(double value) : ValidationRuleAttribute(ComparableSizeMode.Size, value);
