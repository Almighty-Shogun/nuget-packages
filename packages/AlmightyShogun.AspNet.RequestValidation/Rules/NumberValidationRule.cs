namespace AlmightyShogun.AspNet.RequestValidation;

/// <summary>
/// Checks that a value is numeric, integral, of a given scale, or a multiple of something.
/// </summary>
///
/// <author>Almighty-Shogun</author>
/// <since>Unreleased</since>
internal sealed class NumberValidationRule<TRequest, TProperty>(
    NumberMode mode,
    decimal value = 0
) : IPropertyValidationRule<TRequest, TProperty> where TRequest : class
{
    /// <inheritdoc />
    public ValueTask<ValidationRuleResult> ValidateAsync(
        TRequest request,
        TProperty? propertyValue,
        string field,
        IServiceProvider serviceProvider,
        CancellationToken cancellationToken = default
    )
    {
        if (ValidationValue.IsEmpty(propertyValue))
            return ValueTask.FromResult(ValidationRuleResult.Success());

        ValidationRuleResult result = mode switch
        {
            NumberMode.Numeric => ValidationValue.IsNumeric(propertyValue)
                ? ValidationRuleResult.Success()
                : ValidationRuleResult.Failure("validation.numeric"),
            NumberMode.Integer => ValidationValue.IsInteger(propertyValue)
                ? ValidationRuleResult.Success()
                : ValidationRuleResult.Failure("validation.integer"),
            NumberMode.DecimalPlaces => ValidateDecimalPlaces(propertyValue),
            NumberMode.MultipleOf => ValidateMultipleOf(propertyValue),
            _ => throw new InvalidOperationException($"Unsupported NumberMode value '{mode}'.")
        };

        return ValueTask.FromResult(result);
    }

    /// <summary>
    /// Validates that a numeric value has the configured decimal place count.
    /// </summary>
    ///
    /// <param name="propertyValue">The property value.</param>
    ///
    /// <returns>
    /// The result, reporting the scale failure both when the count is wrong and when no scale could be read at all, which is the case for
    /// text that is not a number and for a floating value no decimal can hold.
    /// </returns>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    private ValidationRuleResult ValidateDecimalPlaces(TProperty? propertyValue)
    {
        if (!ValidationValue.TryGetDecimalPlaces(propertyValue, out int actualPlaces))
            return ValidationRuleResult.Failure("validation.decimal", (int)value);

        return actualPlaces == (int)value
            ? ValidationRuleResult.Success()
            : ValidationRuleResult.Failure("validation.decimal", (int)value);
    }

    /// <summary>
    /// Validates that a numeric value is a multiple of the configured value.
    /// </summary>
    ///
    /// <param name="propertyValue">The property value.</param>
    ///
    /// <returns>
    /// The result. A value no decimal can hold exactly, such as an infinity or a double beyond decimal's range, fails the rule rather
    /// than being reported as non-numeric: it is a number, and it is not a multiple of anything checkable. A configured divisor of zero
    /// fails every value, since nothing is a multiple of it.
    /// </returns>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    private ValidationRuleResult ValidateMultipleOf(TProperty? propertyValue)
    {
        if (!ValidationValue.TryGetNumber(propertyValue, out decimal number))
            return ValidationRuleResult.Failure("validation.multiple-of", value);

        return value != 0 && number % value == 0
            ? ValidationRuleResult.Success()
            : ValidationRuleResult.Failure("validation.multiple-of", value);
    }
}
