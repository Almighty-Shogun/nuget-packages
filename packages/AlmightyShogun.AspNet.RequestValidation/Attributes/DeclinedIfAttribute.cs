using System.Reflection;

namespace AlmightyShogun.AspNet.RequestValidation;

/// <summary>
/// Requires the field to contain a declined value when another field equals one of the provided values. Once the condition matches, an
/// absent or empty value fails: the requirement is not skipped for a field that was left out.
/// </summary>
///
/// <param name="field">The request field that controls whether this field must be declined.</param>
/// <param name="values">The values that trigger declined validation.</param>
///
/// <author>Almighty-Shogun</author>
/// <since>Unreleased</since>
[AttributeUsage(AttributeTargets.Property)]
public sealed class DeclinedIfAttribute(string field, params object?[] values) : ValidationRuleAttribute
{
    /// <inheritdoc />
    internal override IPropertyValidationRule<TRequest, TProperty> CreateRule<TRequest, TProperty>(PropertyInfo property)
        => new NamedConditionalValidationRule<TRequest, TProperty>(ConditionalTargetMode.Declined, ConditionMode.If, field, values);
}
