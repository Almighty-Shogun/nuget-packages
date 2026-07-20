using System.Reflection;

namespace AlmightyShogun.AspNet.Validation;

/// <summary>
/// Declares the multiple of validation rule for a request property.
/// </summary>
///
/// <param name="value">The number that the property value must be a multiple of.</param>
///
/// <author>Almighty-Shogun</author>
/// <since>Unreleased</since>
[AttributeUsage(AttributeTargets.Property)]
public sealed class MultipleOfAttribute(double value) : ValidationRuleAttribute
{
    /// <inheritdoc />
    internal override IPropertyValidationRule<TRequest, TProperty> CreateRule<TRequest, TProperty>(PropertyInfo property)
        => new NumberValidationRule<TRequest, TProperty>(NumberMode.MultipleOf, (decimal)value);
}
