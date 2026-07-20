namespace AlmightyShogun.AspNet.Validation;

/// <summary>
/// Validates comparable size constraints for a request property.
/// </summary>
///
/// <author>Almighty-Shogun</author>
/// <since>Unreleased</since>
internal sealed class ComparableSizeValidationRule<TRequest, TProperty>(ComparableSizeMode mode, decimal value, decimal? maxValue = null) : IPropertyValidationRule<TRequest, TProperty> where TRequest : class
{
    /// <inheritdoc />
    public ValueTask<ValidationRuleResult> ValidateAsync(TRequest request, TProperty? propertyValue, string field, IServiceProvider serviceProvider, CancellationToken cancellationToken = default)
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
    /// Gets the validation message key for the configured size comparison mode.
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
    /// Gets the validation message parameters for the configured size comparison mode.
    /// </summary>
    ///
    /// <returns>The validation message parameters.</returns>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    private object?[] GetMessageParameters() => mode == ComparableSizeMode.Between ? [value, maxValue] : [value];
}
