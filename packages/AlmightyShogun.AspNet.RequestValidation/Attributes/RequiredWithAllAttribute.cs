using System.Reflection;

namespace AlmightyShogun.AspNet.RequestValidation;

/// <summary>
/// Requires the field when all listed fields are present. Presence rules run before value rules, so a field this rejects reports that
/// rather than a later format or size failure.
/// </summary>
///
/// <param name="fields">The request fields that trigger required validation when all are present.</param>
///
/// <author>Almighty-Shogun</author>
/// <since>Unreleased</since>
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
