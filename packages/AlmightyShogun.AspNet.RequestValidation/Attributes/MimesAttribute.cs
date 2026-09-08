using System.Reflection;

namespace AlmightyShogun.AspNet.RequestValidation;

/// <summary>
/// Allows only files whose content type is one of the provided values, each written either as a content type or as an extension resolved
/// to one. An absent or empty value passes, so pair it with <see cref="RequiredAttribute"/> when the field is mandatory.
/// </summary>
///
/// <param name="mimes">
/// The allowed values. One containing <c>/</c> is taken as a <c>type/subtype</c> content type and permitted as written; anything else is
/// looked up as a file extension, with or without a leading dot, and contributes nothing when the lookup does not recognize it.
/// </param>
///
/// <author>Almighty-Shogun</author>
/// <since>4.0.0</since>
[AttributeUsage(AttributeTargets.Property)]
public sealed class MimesAttribute(params string[] mimes) : ValidationRuleAttribute
{
    /// <inheritdoc />
    internal override IPropertyValidationRule<TRequest, TProperty> CreateRule<TRequest, TProperty>(PropertyInfo property)
        => new FileConstraintValidationRule<TRequest, TProperty>(FileConstraintMode.Mimes, mimes);
}
