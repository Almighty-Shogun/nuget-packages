namespace AlmightyShogun.AspNet.Validation;

/// <summary>
/// Declares the max digits validation rule for a request property.
/// </summary>
///
/// <param name="max">The maximum number of digits allowed.</param>
///
/// <author>Almighty-Shogun</author>
/// <since>Unreleased</since>
[AttributeUsage(AttributeTargets.Property)]
public sealed class MaxDigitsAttribute(int max) : ValidationRuleAttribute(DigitMode.Max, max);
