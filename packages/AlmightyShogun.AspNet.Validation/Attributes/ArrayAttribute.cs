namespace AlmightyShogun.AspNet.Validation;

/// <summary>
/// Declares the array validation rule for a request property.
/// </summary>
///
/// <author>Almighty-Shogun</author>
/// <since>Unreleased</since>
[AttributeUsage(AttributeTargets.Property)]
public sealed class ArrayAttribute() : ValidationRuleAttribute(TypeMode.Array);
