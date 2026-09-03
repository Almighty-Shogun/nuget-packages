using System.Reflection;

namespace AlmightyShogun.AspNet.RequestValidation;

/// <summary>
/// Prohibits the field when another field contains a declined value. Presence rules run before value rules, so a field this rejects reports
/// that rather than a later format or size failure.
/// </summary>
///
/// <param name="field">The request field that triggers prohibited validation when declined.</param>
///
/// <author>Almighty-Shogun</author>
/// <since>Unreleased</since>
[AttributeUsage(AttributeTargets.Property)]
public sealed class ProhibitedIfDeclinedAttribute(string field) : ValidationRuleAttribute
{
    /// <inheritdoc />
    internal override IPropertyValidationRule<TRequest, TProperty> CreateRule<TRequest, TProperty>(PropertyInfo property)
        => new NamedConditionalStateValidationRule<TRequest, TProperty>(
            ConditionalStateTargetMode.Prohibited,
            ConditionalStateMode.Declined,
            field
        );
}
