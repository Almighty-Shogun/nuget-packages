namespace AlmightyShogun.AspNet.RequestValidation;

/// <summary>
/// Requires the value to be greater than the provided number. An absent or empty value passes, so pair it with
/// <see cref="RequiredAttribute"/> when the field is mandatory.
/// </summary>
///
/// <param name="value">The exclusive minimum value.</param>
///
/// <author>Almighty-Shogun</author>
/// <since>Unreleased</since>
[AttributeUsage(AttributeTargets.Property)]
public sealed class GreaterThanAttribute(double value) : ValidationRuleAttribute(ComparableSizeMode.GreaterThan, value);
