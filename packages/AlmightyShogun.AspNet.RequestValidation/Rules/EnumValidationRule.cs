using System.Globalization;

namespace AlmightyShogun.AspNet.RequestValidation;

/// <summary>
/// Requires the value to be one the enum actually defines, which a cast alone would not guarantee.
/// </summary>
///
/// <author>Almighty-Shogun</author>
/// <since>Unreleased</since>
internal sealed class EnumValidationRule<TRequest, TProperty>(
    Type enumType
) : IPropertyValidationRule<TRequest, TProperty> where TRequest : class
{
    /// <summary>
    /// The enum membership is checked against, unwrapped from <see cref="Nullable{T}"/> so a nullable enum property is checked against
    /// the same set its non-nullable spelling would be.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    private readonly Type _enumType = Nullable.GetUnderlyingType(enumType) ?? enumType;

    /// <inheritdoc />
    public ValueTask<ValidationRuleResult> ValidateAsync(
        TRequest request,
        TProperty? value,
        string field,
        IServiceProvider serviceProvider,
        CancellationToken cancellationToken = default
    )
    {
        if (ValidationValue.IsEmpty(value))
            return ValueTask.FromResult(ValidationRuleResult.Success());

        return ValueTask.FromResult(IsDefinedEnumValue(value)
            ? ValidationRuleResult.Success()
            : ValidationRuleResult.Failure("validation.enum", field));
    }

    /// <summary>
    /// Checks whether a value is defined by the configured enum type.
    /// </summary>
    ///
    /// <param name="value">
    /// The bound value, of whatever type the property declared, so every branch tests the runtime type rather than a cast.
    /// </param>
    ///
    /// <returns><c>true</c> when the value is defined; otherwise, <c>false</c>.</returns>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    private bool IsDefinedEnumValue(object? value)
    {
        if (!_enumType.IsEnum || value is null)
            return false;

        if (value.GetType() == _enumType)
            return Enum.IsDefined(_enumType, value);

        if (value is string text)
            return Enum.TryParse(_enumType, text, false, out object? parsed) && Enum.IsDefined(_enumType, parsed);

        if (!IsIntegral(value))
            return false;

        try
        {
            Type underlyingType = Enum.GetUnderlyingType(_enumType);
            object convertedValue = Convert.ChangeType(value, underlyingType, CultureInfo.InvariantCulture);

            return Enum.IsDefined(_enumType, convertedValue);
        }
        catch (InvalidCastException)
        {
            return false;
        }
        catch (FormatException)
        {
            return false;
        }
        catch (OverflowException)
        {
            return false;
        }
    }

    /// <summary>
    /// Reports whether a value is a whole number, which is the only shape that stands for an enum's underlying value.
    /// </summary>
    ///
    /// <param name="value">The bound value, of whatever type the property declared.</param>
    ///
    /// <returns><c>true</c> for any integral type; otherwise, <c>false</c>.</returns>
    ///
    /// <remarks>
    /// Checked before conversion, because <see cref="Convert.ChangeType(object, Type, IFormatProvider)"/> accepts far more than an enum
    /// should: a <see cref="bool"/> becomes <c>1</c> or <c>0</c> and a fractional number rounds, so either would validate as a member it
    /// never named.
    /// </remarks>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    private static bool IsIntegral(object value) => value is byte or sbyte or short or ushort or int or uint or long or ulong;
}
