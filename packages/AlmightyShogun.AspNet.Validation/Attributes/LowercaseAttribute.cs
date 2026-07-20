namespace AlmightyShogun.AspNet.Validation;

/// <summary>
/// Declares the lowercase validation rule for a request property.
/// </summary>
///
/// <author>Almighty-Shogun</author>
/// <since>Unreleased</since>
[AttributeUsage(AttributeTargets.Property)]
public sealed class LowercaseAttribute() : ValidationRuleAttribute(StringCharacterMode.Lowercase);
