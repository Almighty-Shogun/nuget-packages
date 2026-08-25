namespace AlmightyShogun.AspNet.RequestValidation;

/// <summary>
/// Requires the value to contain at least the provided number of digits. An absent or empty value passes, so pair it with
/// <see cref="RequiredAttribute"/> when the field is mandatory.
/// </summary>
///
/// <param name="min">The minimum number of digits required.</param>
///
/// <author>Almighty-Shogun</author>
/// <since>Unreleased</since>
[AttributeUsage(AttributeTargets.Property)]
public sealed class MinDigitsAttribute(int min) : ValidationRuleAttribute(DigitMode.Min, min);
