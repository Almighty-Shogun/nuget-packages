using System.Reflection;

namespace AlmightyShogun.AspNet.Validation;

/// <summary>
/// Declares the confirmed validation rule for a request property.
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
