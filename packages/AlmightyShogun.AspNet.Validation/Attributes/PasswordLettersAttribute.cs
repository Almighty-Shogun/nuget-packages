using System.Reflection;

namespace AlmightyShogun.AspNet.Validation;

/// <summary>
/// Declares the password letters validation rule for a request property.
/// </summary>
///
/// <author>Almighty-Shogun</author>
/// <since>Unreleased</since>
[AttributeUsage(AttributeTargets.Property)]
public sealed class PasswordLettersAttribute : ValidationRuleAttribute
{
    /// <inheritdoc />
    internal override IPropertyValidationRule<TRequest, TProperty> CreateRule<TRequest, TProperty>(PropertyInfo property)
        => new PasswordValidationRule<TRequest, TProperty>(PasswordRequirement.Letters);
}
