using System.Reflection;

namespace AlmightyShogun.AspNet.RequestValidation;

/// <summary>
/// Requires the field when another field contains a declined value such as <c>false</c> , <c>no</c> , <c>off</c> , or <c>0</c> . Presence
/// rules run before value rules, so a field this rejects reports that rather than a later format or size failure.
/// </summary>
///
/// <param name="field">The request field that triggers required validation when declined.</param>
///
/// <author>Almighty-Shogun</author>
/// <since>Unreleased</since>
[AttributeUsage(AttributeTargets.Property)]
public sealed class RequiredIfDeclinedAttribute(string field) : ValidationRuleAttribute
{
    /// <inheritdoc />
    internal override IPropertyValidationRule<TRequest, TProperty> CreateRule<TRequest, TProperty>(PropertyInfo property)
        => new NamedConditionalStateValidationRule<TRequest, TProperty>(
            ConditionalStateTargetMode.Required,
            ConditionalStateMode.Declined,
            field
        );
}
