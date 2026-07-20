namespace AlmightyShogun.AspNet.Validation;

/// <summary>
/// Declares the contains validation rule for a request property.
/// </summary>
///
/// <param name="values">The values that must be contained.</param>
///
/// <author>Almighty-Shogun</author>
/// <since>Unreleased</since>
[AttributeUsage(AttributeTargets.Property)]
public sealed class ContainsAttribute(params string[] values) : ValidationRuleAttribute(StringMatchMode.Contain, values);
