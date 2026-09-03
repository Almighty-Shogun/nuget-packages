using System.Reflection;

namespace AlmightyShogun.AspNet.RequestValidation;

/// <summary>
/// Allows only files with one of the provided MIME types. An absent or empty value passes, so pair it with <see cref="RequiredAttribute"/>
/// when the field is mandatory.
/// </summary>
///
/// <param name="mimeTypes">The allowed MIME types.</param>
///
/// <author>Almighty-Shogun</author>
/// <since>Unreleased</since>
[AttributeUsage(AttributeTargets.Property)]
public sealed class MimeTypesAttribute(params string[] mimeTypes) : ValidationRuleAttribute
{
    /// <inheritdoc />
    internal override IPropertyValidationRule<TRequest, TProperty> CreateRule<TRequest, TProperty>(PropertyInfo property)
        => new FileConstraintValidationRule<TRequest, TProperty>(FileConstraintMode.MimeTypes, mimeTypes);
}
