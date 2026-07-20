using System.Globalization;

namespace AlmightyShogun.AspNet.Validation;

/// <summary>
/// Validates enum constraints for a request property.
/// </summary>
///
/// <author>Almighty-Shogun</author>
/// <since>Unreleased</since>
internal sealed class EnumValidationRule<TRequest, TProperty>(Type enumType) : IPropertyValidationRule<TRequest, TProperty> where TRequest : class
{
    private readonly Type _enumType = Nullable.GetUnderlyingType(enumType) ?? enumType;

    /// <inheritdoc />
    public ValueTask<ValidationRuleResult> ValidateAsync(TRequest request, TProperty? value, string field, IServiceProvider serviceProvider, CancellationToken cancellationToken = default)
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
    /// <param name="value">The value to check.</param>
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
}
