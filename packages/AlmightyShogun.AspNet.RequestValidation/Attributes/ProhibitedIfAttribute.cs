using System.Reflection;

namespace AlmightyShogun.AspNet.RequestValidation;

/// <summary>
/// Prohibits the field when another field equals one of the provided values. Presence rules run before value rules, so a field this rejects
/// reports that rather than a later format or size failure.
/// </summary>
///
/// <param name="field">The request field that controls whether this field is prohibited.</param>
/// <param name="values">The values that trigger prohibited validation.</param>
///
/// <author>Almighty-Shogun</author>
/// <since>Unreleased</since>
[AttributeUsage(AttributeTargets.Property)]
public sealed class ProhibitedIfAttribute(string field, params object?[] values) : ValidationRuleAttribute
{
    /// <inheritdoc />
    internal override IPropertyValidationRule<TRequest, TProperty> CreateRule<TRequest, TProperty>(PropertyInfo property)
        => new NamedConditionalValidationRule<TRequest, TProperty>(ConditionalTargetMode.Prohibited, ConditionMode.If, field, values);
}
