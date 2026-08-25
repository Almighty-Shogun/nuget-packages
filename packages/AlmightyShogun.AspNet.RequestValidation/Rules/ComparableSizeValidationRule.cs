namespace AlmightyShogun.AspNet.RequestValidation;

/// <summary>
/// Compares a measured size against one or two bounds. What is measured depends on the bound type, so one rule serves numbers, strings,
/// collections, and uploads.
/// </summary>
///
/// <author>Almighty-Shogun</author>
/// <since>Unreleased</since>
internal sealed class ComparableSizeValidationRule<TRequest, TProperty>(
    ComparableSizeMode mode,
    decimal value,
    decimal? maxValue = null
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

        if (!ValidationValue.TryGetComparableSize(propertyValue, out decimal size, out ValidationValueType type))
            return ValueTask.FromResult(ValidationRuleResult.Failure("validation.numeric"));

        bool isValid = mode switch
        {
            ComparableSizeMode.Min => size >= value,
            ComparableSizeMode.Max => size <= value,
            ComparableSizeMode.Between => size >= value && size <= maxValue,
            ComparableSizeMode.Size => size == value,
            ComparableSizeMode.GreaterThan => size > value,
            ComparableSizeMode.GreaterThanOrEqual => size >= value,
            ComparableSizeMode.LessThan => size < value,
            ComparableSizeMode.LessThanOrEqual => size <= value,
            _ => false
        };

        string messageType = ValidationValue.ToMessageType(type);

        return ValueTask.FromResult(isValid
            ? ValidationRuleResult.Success()
            : ValidationRuleResult.Failure(GetMessageKey(messageType), GetMessageParameters()));
    }

    /// <summary>
    /// Maps the configured mode onto the message key its failure reports, so one rule class serves every spelling of its family without
    /// each needing a class of its own.
    /// </summary>
    ///
    /// <param name="messageType">The size message type segment.</param>
    ///
    /// <returns>The validation message key.</returns>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    private string GetMessageKey(string messageType) => mode switch
    {
        ComparableSizeMode.Min => $"validation.min.{messageType}",
        ComparableSizeMode.Max => $"validation.max.{messageType}",
        ComparableSizeMode.Between => $"validation.between.{messageType}",
        ComparableSizeMode.Size => $"validation.size.{messageType}",
        ComparableSizeMode.GreaterThan => $"validation.greater-than.{messageType}",
        ComparableSizeMode.GreaterThanOrEqual => $"validation.greater-than-or-equal.{messageType}",
        ComparableSizeMode.LessThan => $"validation.less-than.{messageType}",
        _ => $"validation.less-than-or-equal.{messageType}"
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
    private object?[] GetMessageParameters() => mode == ComparableSizeMode.Between ? [value, maxValue] : [value];
}
