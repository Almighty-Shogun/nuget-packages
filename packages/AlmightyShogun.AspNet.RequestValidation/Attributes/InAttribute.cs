using System.Reflection;

namespace AlmightyShogun.AspNet.RequestValidation;

/// <summary>
/// Requires the field value to be inside a set of allowed values. An absent or empty value passes, so pair it with
/// <see cref="RequiredAttribute"/> when the field is mandatory.
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

        for (var i = 0; i < values.Length; i++)
            typedValues[i] = (TProperty?)values[i];

        return new SetMembershipValidationRule<TRequest, TProperty>(typedValues, true);
    }
}
