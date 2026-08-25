namespace AlmightyShogun.AspNet.RequestValidation;

/// <summary>
/// Counts the digits a value is written with, rather than comparing the number it spells, so leading zeroes count and a sign does not.
/// </summary>
///
/// <author>Almighty-Shogun</author>
/// <since>Unreleased</since>
internal sealed class DigitCountValidationRule<TRequest, TProperty>(
    DigitMode mode,
    int value,
    int? maxValue = null
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

        if (!ValidationValue.TryGetDigitText(propertyValue, out string text))
            return ValueTask.FromResult(ValidationRuleResult.Failure(GetMessageKey(), GetMessageParameters()));

        bool isValid = mode switch
        {
            DigitMode.Exact => text.Length == value,
            DigitMode.Between => text.Length >= value && text.Length <= maxValue,
            DigitMode.Min => text.Length >= value,
            DigitMode.Max => text.Length <= value,
            _ => false
        };

        return ValueTask.FromResult(isValid
            ? ValidationRuleResult.Success()
            : ValidationRuleResult.Failure(GetMessageKey(), GetMessageParameters()));
    }

    /// <summary>
    /// Maps the configured mode onto the message key its failure reports, so one rule class serves every spelling of its family without
    /// each needing a class of its own.
    /// </summary>
    ///
    /// <returns>The validation message key.</returns>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    private string GetMessageKey() => mode switch
    {
        DigitMode.Exact => "validation.digits",
        DigitMode.Between => "validation.digits.between",
        DigitMode.Min => "validation.min.digits",
        _ => "validation.max.digits"
    };

    /// <summary>
    /// Maps the configured mode onto the values a message template substitutes, so the bounds a rule was built with appear in the sentence
    /// the client reads.
    /// </summary>
    ///
    /// <returns>The validation message parameters.</returns>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    private object?[] GetMessageParameters() => mode == DigitMode.Between ? [value, maxValue] : [value];
}
