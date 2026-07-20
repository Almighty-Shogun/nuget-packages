namespace AlmightyShogun.AspNet.Validation;

/// <summary>
/// Declares the min digits validation rule for a request property.
/// </summary>
///
/// <param name="min">The minimum number of digits required.</param>
///
/// <author>Almighty-Shogun</author>
/// <since>Unreleased</since>
[AttributeUsage(AttributeTargets.Property)]
public sealed class MinDigitsAttribute(int min) : ValidationRuleAttribute(DigitMode.Min, min);
