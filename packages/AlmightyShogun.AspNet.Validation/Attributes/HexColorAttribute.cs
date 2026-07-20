namespace AlmightyShogun.AspNet.Validation;

/// <summary>
/// Declares the hex color validation rule for a request property.
/// </summary>
///
/// <author>Almighty-Shogun</author>
/// <since>Unreleased</since>
[AttributeUsage(AttributeTargets.Property)]
public sealed class HexColorAttribute() : ValidationRuleAttribute(FormatMode.HexColor);
