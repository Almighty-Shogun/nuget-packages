namespace AlmightyShogun.AspNet.RequestValidation;

/// <summary>
/// Requires the value to be greater than or equal to the provided number. An absent or empty value passes, so pair it with
/// <see cref="RequiredAttribute"/> when the field is mandatory.
/// </summary>
///
/// <param name="value">The inclusive minimum value.</param>
///
/// <author>Almighty-Shogun</author>
/// <since>Unreleased</since>
[AttributeUsage(AttributeTargets.Property)]
public sealed class GreaterThanOrEqualAttribute(double value) : ValidationRuleAttribute(ComparableSizeMode.GreaterThanOrEqual, value);
