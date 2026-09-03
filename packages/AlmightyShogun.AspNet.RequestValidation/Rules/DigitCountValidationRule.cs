namespace AlmightyShogun.AspNet.RequestValidation;

/// <summary>
/// Counts the digits a value is written with, rather than comparing the number it spells, so a leading zero counts as the digit it is.
/// </summary>
///
/// <remarks>
/// Only a run of ASCII digits is counted. A sign, a decimal point, or a thousands separator is not skipped over: a value carrying one is
/// not a digit run at all and fails the rule outright rather than having its digits counted.
/// </remarks>
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
            _ => throw new InvalidOperationException($"Unsupported DigitMode value '{mode}'.")
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
        DigitMode.Max => "validation.max.digits",
        _ => throw new InvalidOperationException($"Unsupported DigitMode value '{mode}'.")
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
