using System.Reflection;

namespace AlmightyShogun.AspNet.Validation;

/// <summary>
/// Declares the required array keys validation rule for a request property.
/// </summary>
///
/// <param name="keys">The array keys that must be present.</param>
///
/// <author>Almighty-Shogun</author>
/// <since>Unreleased</since>
[AttributeUsage(AttributeTargets.Property)]
public sealed class RequiredArrayKeysAttribute(params string[] keys) : ValidationRuleAttribute
{
    /// <inheritdoc />
    internal override IPropertyValidationRule<TRequest, TProperty> CreateRule<TRequest, TProperty>(PropertyInfo property)
        => new ArrayKeysValidationRule<TRequest, TProperty>(ArrayKeyMode.AllRequiredKeys, keys);
}
