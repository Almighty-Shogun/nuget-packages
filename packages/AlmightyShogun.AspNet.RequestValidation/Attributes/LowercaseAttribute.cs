namespace AlmightyShogun.AspNet.RequestValidation;

/// <summary>
/// Requires the text value to already be lowercase. An absent or empty value passes, so pair it with <see cref="RequiredAttribute"/> when
/// the field is mandatory.
/// </summary>
///
/// <author>Almighty-Shogun</author>
/// <since>Unreleased</since>
[AttributeUsage(AttributeTargets.Property)]
public sealed class LowercaseAttribute() : ValidationRuleAttribute(StringCharacterMode.Lowercase);
