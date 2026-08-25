namespace AlmightyShogun.AspNet.RequestValidation;

/// <summary>
/// Requires the value to contain only letters, numbers, dashes, and underscores. Use it for slugs, handles, and similar identifier-style
/// text. An absent or empty value passes, so pair it with <see cref="RequiredAttribute"/> when the field is mandatory.
/// </summary>
///
/// <author>Almighty-Shogun</author>
/// <since>Unreleased</since>
[AttributeUsage(AttributeTargets.Property)]
public sealed class AlphaDashAttribute() : ValidationRuleAttribute(StringCharacterMode.AlphaDash);
