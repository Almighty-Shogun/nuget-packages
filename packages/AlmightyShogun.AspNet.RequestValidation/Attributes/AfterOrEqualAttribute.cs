using System.Reflection;

namespace AlmightyShogun.AspNet.RequestValidation;

/// <summary>
/// Requires the value to be after or equal to a literal date or another request field.
/// An absent or empty value passes, so pair it with <see cref="RequiredAttribute"/> when the field is mandatory.
/// </summary>
///
/// <param name="target">The comparison date value or request field name. A date value written with no offset is read as UTC.</param>
/// <param name="targetType">Whether the target is a literal value or a request field.</param>
///
/// <author>Almighty-Shogun</author>
/// <since>4.0.0</since>
[AttributeUsage(AttributeTargets.Property)]
public sealed class AfterOrEqualAttribute(string target, ComparisonTarget targetType = ComparisonTarget.Value) : ValidationRuleAttribute
{
    /// <inheritdoc />
    internal override IPropertyValidationRule<TRequest, TProperty> CreateRule<TRequest, TProperty>(PropertyInfo property)
        => targetType == ComparisonTarget.Field
            ? new DateValidationRule<TRequest, TProperty>(DateMode.AfterOrEqual, target)
            : new DateValidationRule<TRequest, TProperty>(DateMode.AfterOrEqual, ValidationDate.ParseTargetDate(target));
}
