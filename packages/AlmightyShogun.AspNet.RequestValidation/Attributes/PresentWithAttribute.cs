using System.Reflection;

namespace AlmightyShogun.AspNet.RequestValidation;

/// <summary>
/// Requires the field to be present when any listed field is present. Presence rules run before value rules, so a field this rejects
/// reports that rather than a later format or size failure.
/// </summary>
///
/// <param name="fields">The request fields that trigger present validation when any are present.</param>
///
/// <author>Almighty-Shogun</author>
/// <since>Unreleased</since>
[AttributeUsage(AttributeTargets.Property)]
public sealed class PresentWithAttribute(params string[] fields) : ValidationRuleAttribute
{
    /// <inheritdoc />
    internal override IPropertyValidationRule<TRequest, TProperty> CreateRule<TRequest, TProperty>(PropertyInfo property)
        => new NamedMultiFieldPresenceValidationRule<TRequest, TProperty>(
            MultiFieldPresenceTargetMode.Present,
            MultiFieldPresenceTriggerMode.WithAny,
            fields
        );
}
