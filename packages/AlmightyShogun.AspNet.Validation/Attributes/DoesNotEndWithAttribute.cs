namespace AlmightyShogun.AspNet.Validation;

/// <summary>
/// Declares the does not end with validation rule for a request property.
/// </summary>
///
/// <param name="values">The suffixes that the value must not end with.</param>
///
/// <author>Almighty-Shogun</author>
/// <since>Unreleased</since>
[AttributeUsage(AttributeTargets.Property)]
public sealed class DoesNotEndWithAttribute(params string[] values)
    : ValidationRuleAttribute(StringMatchMode.EndWith, values, true);
