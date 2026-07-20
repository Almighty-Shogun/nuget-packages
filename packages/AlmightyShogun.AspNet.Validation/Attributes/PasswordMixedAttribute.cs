using System.Reflection;

namespace AlmightyShogun.AspNet.Validation;

/// <summary>
/// Declares the password mixed validation rule for a request property.
/// </summary>
///
/// <author>Almighty-Shogun</author>
/// <since>Unreleased</since>
[AttributeUsage(AttributeTargets.Property)]
public sealed class PasswordMixedAttribute : ValidationRuleAttribute
{
    /// <inheritdoc />
    internal override IPropertyValidationRule<TRequest, TProperty> CreateRule<TRequest, TProperty>(PropertyInfo property)
        => new PasswordValidationRule<TRequest, TProperty>(PasswordRequirement.Mixed);
}
