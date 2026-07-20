using System.Reflection;

namespace AlmightyShogun.AspNet.Validation;

/// <summary>
/// Declares the in array validation rule for a request property.
/// </summary>
///
/// <param name="field">The other request field whose values must contain this value.</param>
///
/// <author>Almighty-Shogun</author>
/// <since>Unreleased</since>
[AttributeUsage(AttributeTargets.Property)]
public sealed class InArrayAttribute(string field) : ValidationRuleAttribute
{
    /// <inheritdoc />
    internal override IPropertyValidationRule<TRequest, TProperty> CreateRule<TRequest, TProperty>(PropertyInfo property)
        => new InArrayValidationRule<TRequest, TProperty>(ValidationField<TRequest>.FromPropertyName(field));
}
