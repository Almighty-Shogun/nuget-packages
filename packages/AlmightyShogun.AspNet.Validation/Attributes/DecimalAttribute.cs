using System.Reflection;

namespace AlmightyShogun.AspNet.Validation;

/// <summary>
/// Declares the decimal validation rule for a request property.
/// </summary>
///
/// <param name="places">The exact number of decimal places required.</param>
///
/// <author>Almighty-Shogun</author>
/// <since>Unreleased</since>
[AttributeUsage(AttributeTargets.Property)]
public sealed class DecimalAttribute(int places) : ValidationRuleAttribute
{
    /// <inheritdoc />
    internal override IPropertyValidationRule<TRequest, TProperty> CreateRule<TRequest, TProperty>(PropertyInfo property)
        => new NumberValidationRule<TRequest, TProperty>(NumberMode.DecimalPlaces, places);
}
