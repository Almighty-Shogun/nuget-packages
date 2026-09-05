using System.Reflection;

namespace AlmightyShogun.AspNet.RequestValidation;

/// <summary>
/// Makes this field prohibit the listed fields from being present with a value. Use it for mutually exclusive request options.
/// </summary>
///
/// <param name="fields">The request fields the rule reads alongside this one.</param>
///
/// <author>Almighty-Shogun</author>
/// <since>4.0.0</since>
[AttributeUsage(AttributeTargets.Property)]
public sealed class ProhibitsAttribute(params string[] fields) : ValidationRuleAttribute
{
    /// <inheritdoc />
    internal override IPropertyValidationRule<TRequest, TProperty> CreateRule<TRequest, TProperty>(PropertyInfo property)
        => new NamedMultiFieldPresenceValidationRule<TRequest, TProperty>(
            MultiFieldPresenceTargetMode.Prohibits,
            MultiFieldPresenceTriggerMode.Prohibits,
            fields
        );
}
