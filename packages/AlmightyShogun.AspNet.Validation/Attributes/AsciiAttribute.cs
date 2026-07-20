namespace AlmightyShogun.AspNet.Validation;

/// <summary>
/// Declares the ASCII validation rule for a request property.
/// </summary>
///
/// <author>Almighty-Shogun</author>
/// <since>Unreleased</since>
[AttributeUsage(AttributeTargets.Property)]
public sealed class AsciiAttribute() : ValidationRuleAttribute(StringCharacterMode.Ascii);
