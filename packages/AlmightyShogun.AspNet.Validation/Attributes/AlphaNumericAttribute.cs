namespace AlmightyShogun.AspNet.Validation;

/// <summary>
/// Declares the alphanumeric validation rule for a request property.
/// </summary>
///
/// <author>Almighty-Shogun</author>
/// <since>Unreleased</since>
[AttributeUsage(AttributeTargets.Property)]
public sealed class AlphaNumericAttribute() : ValidationRuleAttribute(StringCharacterMode.AlphaNumeric);
