using System.Reflection;

namespace AlmightyShogun.AspNet.RequestValidation;

/// <summary>
/// Requires the value to be a string. Only <c>null</c> passes without being one, so pair it with <see cref="RequiredAttribute"/> when the
/// field is mandatory; an empty collection or a zero-length upload fails rather than counting as absent.
/// </summary>
///
/// <author>Almighty-Shogun</author>
/// <since>4.0.0</since>
[AttributeUsage(AttributeTargets.Property)]
public sealed class StringAttribute : ValidationRuleAttribute
{
    /// <inheritdoc />
    internal override IPropertyValidationRule<TRequest, TProperty> CreateRule<TRequest, TProperty>(PropertyInfo property)
        => new TypeValidationRule<TRequest, TProperty>(TypeMode.String);
}
