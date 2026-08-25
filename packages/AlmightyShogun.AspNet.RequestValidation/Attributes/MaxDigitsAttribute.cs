namespace AlmightyShogun.AspNet.RequestValidation;

/// <summary>
/// Allows the value to contain at most the provided number of digits. An absent or empty value passes, so pair it with
/// <see cref="RequiredAttribute"/> when the field is mandatory.
/// </summary>
///
/// <param name="max">The maximum number of digits allowed.</param>
///
/// <author>Almighty-Shogun</author>
/// <since>Unreleased</since>
[AttributeUsage(AttributeTargets.Property)]
public sealed class MaxDigitsAttribute(int max) : ValidationRuleAttribute(DigitMode.Max, max);
