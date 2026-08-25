using System.Reflection;

namespace AlmightyShogun.AspNet.RequestValidation;

/// <summary>
/// Requires an object or dictionary-like field to contain at least one of the provided keys. An absent or empty value passes, so pair it
/// with <see cref="RequiredAttribute"/> when the field is mandatory.
/// </summary>
///
/// <param name="keys">The array keys that may satisfy the rule.</param>
///
/// <author>Almighty-Shogun</author>
/// <since>Unreleased</since>
[AttributeUsage(AttributeTargets.Property)]
public sealed class InArrayKeysAttribute(params string[] keys) : ValidationRuleAttribute
{
    /// <inheritdoc />
    internal override IPropertyValidationRule<TRequest, TProperty> CreateRule<TRequest, TProperty>(PropertyInfo property)
        => new ArrayKeysValidationRule<TRequest, TProperty>(ArrayKeyMode.AnyRequiredKey, keys);
}
