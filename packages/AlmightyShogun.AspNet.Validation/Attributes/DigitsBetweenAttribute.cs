namespace AlmightyShogun.AspNet.Validation;

/// <summary>
/// Declares the digits between validation rule for a request property.
/// </summary>
///
/// <param name="min">The minimum number of digits required.</param>
/// <param name="max">The maximum number of digits allowed.</param>
///
/// <author>Almighty-Shogun</author>
/// <since>Unreleased</since>
[AttributeUsage(AttributeTargets.Property)]
public sealed class DigitsBetweenAttribute(int min, int max) : ValidationRuleAttribute(DigitMode.Between, min, max);
