using System.Reflection;

namespace AlmightyShogun.AspNet.Validation;

/// <summary>
/// Declares the enum validation rule for a request property.
/// </summary>
///
/// <param name="enumType">The enum type to validate against. When omitted, the property type is used.</param>
///
/// <author>Almighty-Shogun</author>
/// <since>Unreleased</since>
[AttributeUsage(AttributeTargets.Property)]
public sealed class EnumAttribute(Type? enumType = null) : ValidationRuleAttribute
{
    /// <inheritdoc />
    internal override IPropertyValidationRule<TRequest, TProperty> CreateRule<TRequest, TProperty>(PropertyInfo property)
        => new EnumValidationRule<TRequest, TProperty>(enumType ?? property.PropertyType);
}

/// <summary>
/// Declares the enum validation rule for a request property using a generic enum type.
/// </summary>
///
/// <typeparam name="TEnum">The enum type to validate against.</typeparam>
///
/// <author>Almighty-Shogun</author>
/// <since>Unreleased</since>
[AttributeUsage(AttributeTargets.Property)]
public sealed class EnumAttribute<TEnum> : ValidationRuleAttribute where TEnum : struct, Enum
{
    /// <inheritdoc />
    internal override IPropertyValidationRule<TRequest, TProperty> CreateRule<TRequest, TProperty>(PropertyInfo property)
        => new EnumValidationRule<TRequest, TProperty>(typeof(TEnum));
}
