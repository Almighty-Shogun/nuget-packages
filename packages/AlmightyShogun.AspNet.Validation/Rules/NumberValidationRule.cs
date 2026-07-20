namespace AlmightyShogun.AspNet.Validation;

/// <summary>
/// Validates number constraints for a request property.
/// </summary>
///
/// <author>Almighty-Shogun</author>
/// <since>Unreleased</since>
internal sealed class NumberValidationRule<TRequest, TProperty>(NumberMode mode, decimal value = 0) : IPropertyValidationRule<TRequest, TProperty> where TRequest : class
{
    /// <inheritdoc />
    public ValueTask<ValidationRuleResult> ValidateAsync(TRequest request, TProperty? propertyValue, string field, IServiceProvider serviceProvider, CancellationToken cancellationToken = default)
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
            _ => ValidationRuleResult.Failure("validation.numeric")
        };

        return ValueTask.FromResult(result);
    }

    /// <summary>
    /// Validates that a numeric value has the configured decimal place count.
    /// </summary>
    ///
    /// <param name="propertyValue">The property value.</param>
    ///
    /// <returns>The validation rule result.</returns>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    private ValidationRuleResult ValidateDecimalPlaces(TProperty? propertyValue)
    {
        if (!ValidationValue.TryGetDecimalPlaces(propertyValue, out int actualPlaces))
            return ValidationRuleResult.Failure("validation.numeric");

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
    /// <returns>The validation rule result.</returns>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    private ValidationRuleResult ValidateMultipleOf(TProperty? propertyValue)
    {
        if (!ValidationValue.TryGetNumber(propertyValue, out decimal number))
            return ValidationRuleResult.Failure("validation.numeric");

        return value != 0 && number % value == 0
            ? ValidationRuleResult.Success()
            : ValidationRuleResult.Failure("validation.multiple-of", value);
    }
}
