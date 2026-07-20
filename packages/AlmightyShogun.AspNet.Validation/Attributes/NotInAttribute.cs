using System.Reflection;

namespace AlmightyShogun.AspNet.Validation;

/// <summary>
/// Declares the set exclusion validation rule for a request property.
/// </summary>
///
/// <param name="values">The forbidden values.</param>
///
/// <author>Almighty-Shogun</author>
/// <since>Unreleased</since>
[AttributeUsage(AttributeTargets.Property)]
public sealed class NotInAttribute(params object?[] values) : ValidationRuleAttribute
{
    /// <inheritdoc />
    internal override IPropertyValidationRule<TRequest, TProperty> CreateRule<TRequest, TProperty>(PropertyInfo property)
    {
        TProperty?[] typedValues = new TProperty?[values.Length];

        for (int i = 0; i < values.Length; i++)
            typedValues[i] = (TProperty?)values[i];

        return new SetMembershipValidationRule<TRequest, TProperty>(typedValues, false);
    }
}
