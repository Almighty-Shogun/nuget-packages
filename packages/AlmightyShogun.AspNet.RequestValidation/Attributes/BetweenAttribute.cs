using System.Reflection;

namespace AlmightyShogun.AspNet.RequestValidation;

/// <summary>
/// Requires the value to be between the inclusive minimum and maximum. An absent or empty value passes, so pair it with
/// <see cref="RequiredAttribute"/> when the field is mandatory.
/// </summary>
///
/// <param name="min">The inclusive minimum value.</param>
/// <param name="max">The inclusive maximum value.</param>
///
/// <author>Almighty-Shogun</author>
/// <since>Unreleased</since>
[AttributeUsage(AttributeTargets.Property)]
public sealed class BetweenAttribute(double min, double max) : ValidationRuleAttribute
{
    /// <inheritdoc />
    internal override IPropertyValidationRule<TRequest, TProperty> CreateRule<TRequest, TProperty>(PropertyInfo property)
        => new ComparableSizeValidationRule<TRequest, TProperty>(ComparableSizeMode.Between, (decimal)min, (decimal)max);
}
