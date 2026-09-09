using System.Reflection;

namespace AlmightyShogun.AspNet.RequestValidation;

/// <summary>
/// Requires the field to be present when another field equals one of the provided values.
/// The field may still be empty unless another rule rejects empty values.
/// </summary>
///
/// <param name="field">The request field the condition reads.</param>
/// <param name="values">The values the controlling field is matched against.</param>
///
/// <author>Almighty-Shogun</author>
/// <since>4.0.0</since>
[AttributeUsage(AttributeTargets.Property)]
public sealed class PresentIfAttribute(string field, params object?[] values) : ValidationRuleAttribute
{
    /// <inheritdoc />
    internal override IPropertyValidationRule<TRequest, TProperty> CreateRule<TRequest, TProperty>(PropertyInfo property)
        => new NamedConditionalValidationRule<TRequest, TProperty>(ConditionalTargetMode.Present, ConditionMode.If, field, values);
}
