using System.Reflection;

namespace AlmightyShogun.AspNet.RequestValidation;

/// <summary>
/// Requires the field when every listed field holds a non-empty value. A listed field posted blank, as an empty collection, or as a
/// zero-length upload does not trigger it.
/// </summary>
///
/// <param name="fields">The request fields the trigger is evaluated against.</param>
///
/// <author>Almighty-Shogun</author>
/// <since>4.0.0</since>
[AttributeUsage(AttributeTargets.Property)]
public sealed class RequiredWithAllAttribute(params string[] fields) : ValidationRuleAttribute
{
    /// <inheritdoc />
    internal override IPropertyValidationRule<TRequest, TProperty> CreateRule<TRequest, TProperty>(PropertyInfo property)
        => new NamedMultiFieldPresenceValidationRule<TRequest, TProperty>(
            MultiFieldPresenceTargetMode.Required,
            MultiFieldPresenceTriggerMode.WithAll,
            fields
        );
}
