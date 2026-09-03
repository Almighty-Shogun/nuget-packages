using System.Reflection;

namespace AlmightyShogun.AspNet.RequestValidation;

/// <summary>
/// Requires the value to match the exact numeric value, string length, collection count, or file size. An absent or empty value passes, so
/// pair it with <see cref="RequiredAttribute"/> when the field is mandatory.
/// </summary>
///
/// <param name="value">The size the value must equal, measured by whichever quantity the bound type decides.</param>
///
/// <author>Almighty-Shogun</author>
/// <since>Unreleased</since>
[AttributeUsage(AttributeTargets.Property)]
public sealed class SizeAttribute(double value) : ValidationRuleAttribute
{
    /// <inheritdoc />
    internal override IPropertyValidationRule<TRequest, TProperty> CreateRule<TRequest, TProperty>(PropertyInfo property)
        => new ComparableSizeValidationRule<TRequest, TProperty>(ComparableSizeMode.Size, (decimal)value);
}
