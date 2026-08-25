namespace AlmightyShogun.AspNet.RequestValidation;

/// <summary>
/// Rejects text that ends with one of the provided suffixes. An absent or empty value passes, so pair it with
/// <see cref="RequiredAttribute"/> when the field is mandatory.
/// </summary>
///
/// <param name="values">The suffixes that the value must not end with.</param>
///
/// <author>Almighty-Shogun</author>
/// <since>Unreleased</since>
[AttributeUsage(AttributeTargets.Property)]
public sealed class DoesNotEndWithAttribute(params string[] values) : ValidationRuleAttribute(StringMatchMode.EndWith, values, true);
