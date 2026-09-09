using System.Reflection;

namespace AlmightyShogun.AspNet.RequestValidation;

/// <summary>
/// Requires the field to measure below the provided number.
/// An absent or empty value passes, so pair it with <see cref="RequiredAttribute"/> when the field is mandatory.
/// </summary>
///
/// <param name="value">The exclusive maximum value.</param>
///
/// <author>Almighty-Shogun</author>
/// <since>4.0.0</since>
[AttributeUsage(AttributeTargets.Property)]
public sealed class LessThanAttribute(double value) : ValidationRuleAttribute
{
    /// <inheritdoc />
    internal override IPropertyValidationRule<TRequest, TProperty> CreateRule<TRequest, TProperty>(PropertyInfo property)
        => new ComparableSizeValidationRule<TRequest, TProperty>(ComparableSizeMode.LessThan, (decimal)value);
}
