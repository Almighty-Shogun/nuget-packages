using System.Reflection;

namespace AlmightyShogun.AspNet.Validation;

/// <summary>
/// Declares the date format validation rule for a request property.
/// </summary>
///
/// <param name="format">The required date format.</param>
///
/// <author>Almighty-Shogun</author>
/// <since>Unreleased</since>
[AttributeUsage(AttributeTargets.Property)]
public sealed class DateFormatAttribute(string format) : ValidationRuleAttribute
{
    /// <inheritdoc />
    internal override IPropertyValidationRule<TRequest, TProperty> CreateRule<TRequest, TProperty>(PropertyInfo property)
        => new DateValidationRule<TRequest, TProperty>(format);
}
