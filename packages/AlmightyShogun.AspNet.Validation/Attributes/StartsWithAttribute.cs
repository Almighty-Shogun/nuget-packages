namespace AlmightyShogun.AspNet.Validation;

/// <summary>
/// Declares the starts with validation rule for a request property.
/// </summary>
///
/// <param name="values">The prefixes that may satisfy the rule.</param>
///
/// <author>Almighty-Shogun</author>
/// <since>Unreleased</since>
[AttributeUsage(AttributeTargets.Property)]
public sealed class StartsWithAttribute(params string[] values) : ValidationRuleAttribute(StringMatchMode.StartWith, values);
