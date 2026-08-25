namespace AlmightyShogun.AspNet.RequestValidation;

/// <summary>
/// Requires the value to be an array or list-like value. An absent or empty value passes, so pair it with <see cref="RequiredAttribute"/>
/// when the field is mandatory.
/// </summary>
///
/// <author>Almighty-Shogun</author>
/// <since>Unreleased</since>
[AttributeUsage(AttributeTargets.Property)]
public sealed class ListAttribute() : ValidationRuleAttribute(TypeMode.List);
