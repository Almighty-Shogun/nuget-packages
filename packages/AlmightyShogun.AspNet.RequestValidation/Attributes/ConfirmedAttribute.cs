using System.Reflection;

namespace AlmightyShogun.AspNet.RequestValidation;

/// <summary>
/// Requires the field to match a confirmation field. Without an explicit target, the validator uses the property name with
/// <c>Confirmation</c> appended.
/// </summary>
///
/// <param name="field">The confirmation field name. When omitted, the validator uses the property name plus "Confirmation".</param>
///
/// <author>Almighty-Shogun</author>
/// <since>Unreleased</since>
[AttributeUsage(AttributeTargets.Property)]
public sealed class ConfirmedAttribute(string? field = null) : ValidationRuleAttribute
{
    /// <inheritdoc />
    internal override IPropertyValidationRule<TRequest, TProperty> CreateRule<TRequest, TProperty>(PropertyInfo property)
        => field is null
            ? new FieldComparisonValidationRule<TRequest, TProperty, TProperty>()
            : new FieldComparisonValidationRule<TRequest, TProperty, object?>(FieldComparisonMode.Confirmed, field);
}
