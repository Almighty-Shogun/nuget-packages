using System.Reflection;

namespace AlmightyShogun.AspNet.RequestValidation;

/// <summary>
/// Requires the field to contain an accepted value when another field equals one of the provided values. Once the condition matches, an
/// absent or empty value fails: the requirement is not skipped for a field that was left out.
/// </summary>
///
/// <param name="field">The request field that controls whether this field must be accepted.</param>
/// <param name="values">The values that trigger accepted validation.</param>
///
/// <author>Almighty-Shogun</author>
/// <since>Unreleased</since>
[AttributeUsage(AttributeTargets.Property)]
public sealed class AcceptedIfAttribute(string field, params object?[] values) : ValidationRuleAttribute
{
    /// <inheritdoc />
    internal override IPropertyValidationRule<TRequest, TProperty> CreateRule<TRequest, TProperty>(PropertyInfo property)
        => new NamedConditionalValidationRule<TRequest, TProperty>(ConditionalTargetMode.Accepted, ConditionMode.If, field, values);
}
