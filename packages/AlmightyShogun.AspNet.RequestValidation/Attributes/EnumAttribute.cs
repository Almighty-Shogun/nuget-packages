using System.Reflection;

namespace AlmightyShogun.AspNet.RequestValidation;

/// <summary>
/// Requires the value to be parseable as the provided enum type. When the enum type is omitted, the validator uses the request property
/// type. An absent or empty value passes, so pair it with <see cref="RequiredAttribute"/> when the field is mandatory.
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
/// Requires the value to be one the enum defines, with the enum named as a type argument so the compiler checks it rather than a
/// runtime cast. An absent or empty value passes, so pair it with <see cref="RequiredAttribute"/> when the field is mandatory.
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
