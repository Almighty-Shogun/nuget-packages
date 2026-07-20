namespace AlmightyShogun.AspNet.Validation;

/// <summary>
/// Declares the digits validation rule for a request property.
/// </summary>
///
/// <param name="digits">The exact number of digits required.</param>
///
/// <author>Almighty-Shogun</author>
/// <since>Unreleased</since>
[AttributeUsage(AttributeTargets.Property)]
public sealed class DigitsAttribute(int digits) : ValidationRuleAttribute(DigitMode.Exact, digits);
