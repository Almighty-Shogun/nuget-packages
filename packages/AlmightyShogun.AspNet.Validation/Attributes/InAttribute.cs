using System.Reflection;

namespace AlmightyShogun.AspNet.Validation;

/// <summary>
/// Declares the set inclusion validation rule for a request property.
/// </summary>
///
/// <param name="values">The allowed values.</param>
///
/// <author>Almighty-Shogun</author>
/// <since>Unreleased</since>
[AttributeUsage(AttributeTargets.Property)]
public sealed class InAttribute(params object?[] values) : ValidationRuleAttribute
{
    /// <inheritdoc />
    internal override IPropertyValidationRule<TRequest, TProperty> CreateRule<TRequest, TProperty>(PropertyInfo property)
    {
        TProperty?[] typedValues = new TProperty?[values.Length];

        for (int i = 0; i < values.Length; i++)
            typedValues[i] = (TProperty?)values[i];

        return new SetMembershipValidationRule<TRequest, TProperty>(typedValues, true);
    }
}
