namespace AlmightyShogun.AspNet.Validation;

/// <summary>
/// Declares the does not start with validation rule for a request property.
/// </summary>
///
/// <param name="values">The prefixes that the value must not start with.</param>
///
/// <author>Almighty-Shogun</author>
/// <since>Unreleased</since>
[AttributeUsage(AttributeTargets.Property)]
public sealed class DoesNotStartWithAttribute(params string[] values)
    : ValidationRuleAttribute(StringMatchMode.StartWith, values, true);
