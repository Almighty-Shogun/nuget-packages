using System.Reflection;

namespace AlmightyShogun.AspNet.RequestValidation;

/// <summary>
/// Requires the value to be at least the provided number. For strings, collections, and files, the number is interpreted as length, count,
/// or kilobytes. An absent or empty value passes, so pair it with <see cref="RequiredAttribute"/> when the field is mandatory.
/// </summary>
///
/// <param name="value">The inclusive minimum value.</param>
///
/// <author>Almighty-Shogun</author>
/// <since>Unreleased</since>
[AttributeUsage(AttributeTargets.Property)]
public sealed class MinAttribute(double value) : ValidationRuleAttribute
{
    /// <inheritdoc />
    internal override IPropertyValidationRule<TRequest, TProperty> CreateRule<TRequest, TProperty>(PropertyInfo property)
        => new ComparableSizeValidationRule<TRequest, TProperty>(ComparableSizeMode.Min, (decimal)value);
}
