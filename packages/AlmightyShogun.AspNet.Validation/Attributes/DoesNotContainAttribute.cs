namespace AlmightyShogun.AspNet.Validation;

/// <summary>
/// Declares the does not contain validation rule for a request property.
/// </summary>
///
/// <param name="values">The values that must not be contained.</param>
///
/// <author>Almighty-Shogun</author>
/// <since>Unreleased</since>
[AttributeUsage(AttributeTargets.Property)]
public sealed class DoesNotContainAttribute(params string[] values)
    : ValidationRuleAttribute(StringMatchMode.Contain, values, true);
