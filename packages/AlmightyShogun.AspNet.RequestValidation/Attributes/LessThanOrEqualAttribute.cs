namespace AlmightyShogun.AspNet.RequestValidation;

/// <summary>
/// Requires the value to be less than or equal to the provided number. An absent or empty value passes, so pair it with
/// <see cref="RequiredAttribute"/> when the field is mandatory.
/// </summary>
///
/// <param name="value">The inclusive maximum value.</param>
///
/// <author>Almighty-Shogun</author>
/// <since>Unreleased</since>
[AttributeUsage(AttributeTargets.Property)]
public sealed class LessThanOrEqualAttribute(double value) : ValidationRuleAttribute(ComparableSizeMode.LessThanOrEqual, value);
