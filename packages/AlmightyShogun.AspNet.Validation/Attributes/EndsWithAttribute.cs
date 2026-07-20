namespace AlmightyShogun.AspNet.Validation;

/// <summary>
/// Declares the ends with validation rule for a request property.
/// </summary>
///
/// <param name="values">The suffixes that may satisfy the rule.</param>
///
/// <author>Almighty-Shogun</author>
/// <since>Unreleased</since>
[AttributeUsage(AttributeTargets.Property)]
public sealed class EndsWithAttribute(params string[] values) : ValidationRuleAttribute(StringMatchMode.EndWith, values);
