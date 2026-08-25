namespace AlmightyShogun.AspNet.RequestValidation;

/// <summary>
/// Requires the text value to end with one of the provided suffixes. An absent or empty value passes, so pair it with
/// <see cref="RequiredAttribute"/> when the field is mandatory.
/// </summary>
///
/// <param name="values">The suffixes that may satisfy the rule.</param>
///
/// <author>Almighty-Shogun</author>
/// <since>Unreleased</since>
[AttributeUsage(AttributeTargets.Property)]
public sealed class EndsWithAttribute(params string[] values) : ValidationRuleAttribute(StringMatchMode.EndWith, values);
