namespace AlmightyShogun.AspNet.RequestValidation;

/// <summary>
/// Rejects text or collections containing one of the provided values. An absent or empty value passes, so pair it with
/// <see cref="RequiredAttribute"/> when the field is mandatory.
/// </summary>
///
/// <param name="values">The values that must not be contained.</param>
///
/// <author>Almighty-Shogun</author>
/// <since>Unreleased</since>
[AttributeUsage(AttributeTargets.Property)]
public sealed class DoesNotContainAttribute(params string[] values) : ValidationRuleAttribute(StringMatchMode.Contain, values, true);
