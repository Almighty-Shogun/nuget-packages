namespace AlmightyShogun.AspNet.Validation;

/// <summary>
/// Declares the alpha validation rule for a request property.
/// </summary>
///
/// <author>Almighty-Shogun</author>
/// <since>Unreleased</since>
[AttributeUsage(AttributeTargets.Property)]
public sealed class AlphaAttribute() : ValidationRuleAttribute(StringCharacterMode.Alpha);
