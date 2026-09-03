using System.Reflection;

namespace AlmightyShogun.AspNet.RequestValidation;

/// <summary>
/// Requires the value to be an HTTP or HTTPS URL. An absent or empty value passes, so pair it with <see cref="RequiredAttribute"/> when the
/// field is mandatory.
/// </summary>
///
/// <author>Almighty-Shogun</author>
/// <since>Unreleased</since>
[AttributeUsage(AttributeTargets.Property)]
public sealed class UrlAttribute : ValidationRuleAttribute
{
    /// <inheritdoc />
    internal override IPropertyValidationRule<TRequest, TProperty> CreateRule<TRequest, TProperty>(PropertyInfo property)
        => new FormatValidationRule<TRequest, TProperty>(FormatMode.Url);
}
