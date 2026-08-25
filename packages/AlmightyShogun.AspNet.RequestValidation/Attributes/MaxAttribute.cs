namespace AlmightyShogun.AspNet.RequestValidation;

/// <summary>
/// Requires the value to be no greater than the provided number. For strings, collections, and files, the number is interpreted as length,
/// count, or kilobytes. An absent or empty value passes, so pair it with <see cref="RequiredAttribute"/> when the field is mandatory.
/// </summary>
///
/// <param name="value">The inclusive maximum value.</param>
///
/// <author>Almighty-Shogun</author>
/// <since>Unreleased</since>
[AttributeUsage(AttributeTargets.Property)]
public sealed class MaxAttribute(double value) : ValidationRuleAttribute(ComparableSizeMode.Max, value);
