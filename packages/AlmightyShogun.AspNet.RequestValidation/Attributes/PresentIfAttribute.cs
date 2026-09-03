using System.Reflection;

namespace AlmightyShogun.AspNet.RequestValidation;

/// <summary>
/// Requires the field to be present when another field equals one of the provided values. The field may still be empty unless another rule
/// rejects empty values. Presence rules run before value rules, so a field this rejects reports that rather than a later format or size
/// failure.
/// </summary>
///
/// <param name="field">The request field that controls whether this field must be present.</param>
/// <param name="values">The values that trigger present validation.</param>
///
/// <author>Almighty-Shogun</author>
/// <since>Unreleased</since>
[AttributeUsage(AttributeTargets.Property)]
public sealed class PresentIfAttribute(string field, params object?[] values) : ValidationRuleAttribute
{
    /// <inheritdoc />
    internal override IPropertyValidationRule<TRequest, TProperty> CreateRule<TRequest, TProperty>(PropertyInfo property)
        => new NamedConditionalValidationRule<TRequest, TProperty>(ConditionalTargetMode.Present, ConditionMode.If, field, values);
}
