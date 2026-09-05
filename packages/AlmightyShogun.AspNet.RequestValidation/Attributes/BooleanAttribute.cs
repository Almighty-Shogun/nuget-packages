using System.Reflection;

namespace AlmightyShogun.AspNet.RequestValidation;

/// <summary>
/// Requires the value to be a boolean, or text that parses as one. <c>null</c>, an empty string and an empty collection pass, so pair it
/// with <see cref="RequiredAttribute"/> when the field is mandatory; a number and a zero-length upload do not.
/// </summary>
///
/// <author>Almighty-Shogun</author>
/// <since>4.0.0</since>
[AttributeUsage(AttributeTargets.Property)]
public sealed class BooleanAttribute : ValidationRuleAttribute
{
    /// <inheritdoc />
    internal override IPropertyValidationRule<TRequest, TProperty> CreateRule<TRequest, TProperty>(PropertyInfo property)
        => new TypeValidationRule<TRequest, TProperty>(TypeMode.Boolean);
}
