using System.Reflection;

namespace AlmightyShogun.AspNet.RequestValidation;

/// <summary>
/// Requires a dictionary field to carry all the provided keys.
/// An absent or empty value passes, so pair it with <see cref="RequiredAttribute"/> when the field is mandatory.
/// </summary>
///
/// <param name="keys">The keys the field's own keys are checked against.</param>
///
/// <author>Almighty-Shogun</author>
/// <since>4.0.0</since>
[AttributeUsage(AttributeTargets.Property)]
public sealed class RequiredArrayKeysAttribute(params string[] keys) : ValidationRuleAttribute
{
    /// <inheritdoc />
    internal override IPropertyValidationRule<TRequest, TProperty> CreateRule<TRequest, TProperty>(PropertyInfo property)
        => new ArrayKeysValidationRule<TRequest, TProperty>(ArrayKeyMode.AllRequiredKeys, keys);
}
