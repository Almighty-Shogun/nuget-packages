namespace AlmightyShogun.AspNet.RequestValidation;

/// <summary>
/// Checks that a value is numeric, integral, of a given scale, or a multiple of something.
/// An absent or empty value passes without being checked, so the rule never implies the field is required.
/// </summary>
///
/// <typeparam name="TRequest">The request type the rule is declared on, though only the bound value decides the outcome.</typeparam>
/// <typeparam name="TProperty">The bound property's type, read as an object rather than constrained here.</typeparam>
/// <param name="mode">Which numeric check to perform, which also decides the message a failure reports.</param>
/// <param name="value">
/// What the mode reads: the required decimal place count for <see cref="NumberMode.DecimalPlaces"/> and the divisor for
/// <see cref="NumberMode.MultipleOf"/>. Neither of the other two modes reads it.
/// </param>
///
/// <author>Almighty-Shogun</author>
/// <since>4.0.0</since>
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
    /// text that is not a number and for a floating-point value no decimal can hold.
    /// </returns>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>4.0.0</since>
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
    /// <since>4.0.0</since>
    private ValidationRuleResult ValidateMultipleOf(TProperty? propertyValue)
    {
        if (!ValidationValue.TryGetNumber(propertyValue, out decimal number))
            return ValidationRuleResult.Failure("validation.multiple-of", value);

        return value != 0 && number % value == 0
            ? ValidationRuleResult.Success()
            : ValidationRuleResult.Failure("validation.multiple-of", value);
    }
}
