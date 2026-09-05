using System.Reflection;

namespace AlmightyShogun.AspNet.RequestValidation;

/// <summary>
/// Requires the field when another field contains a declined value such as <c>false</c> , <c>no</c> , <c>off</c> , or <c>0</c> .
/// </summary>
///
/// <param name="field">The request field the condition reads.</param>
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
