using System.Reflection;

namespace AlmightyShogun.AspNet.RequestValidation;

/// <summary>
/// Requires the field to be present and contain a non-empty value. It fails for missing values, <c>null</c> , empty text, empty
/// collections, and empty uploaded files. Presence rules run before value rules, so a field this rejects reports that rather than a later
/// format or size failure.
/// </summary>
///
/// <author>Almighty-Shogun</author>
/// <since>Unreleased</since>
[AttributeUsage(AttributeTargets.Property)]
public sealed class RequiredAttribute : ValidationRuleAttribute
{
    /// <inheritdoc />
    internal override IPropertyValidationRule<TRequest, TProperty> CreateRule<TRequest, TProperty>(PropertyInfo property)
        => new PresenceValidationRule<TRequest, TProperty>(PresenceMode.Required);
}
