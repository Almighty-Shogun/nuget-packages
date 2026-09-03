namespace AlmightyShogun.AspNet.RequestValidation;

/// <summary>
/// Compares a measured size against one or two bounds. What is measured depends on the bound type, so one rule serves numbers, strings,
/// collections, and uploads.
/// </summary>
///
/// <author>Almighty-Shogun</author>
/// <since>Unreleased</since>
internal sealed class ComparableSizeValidationRule<TRequest, TProperty> : IPropertyValidationRule<TRequest, TProperty>
    where TRequest : class
{
    /// <summary>
    /// Which comparison this rule performs, and with it which message key a failure reports.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    private readonly ComparableSizeMode _mode;

    /// <summary>
    /// The bound compared against, which is the only bound for every mode but the range one.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    private readonly decimal _value;

    /// <summary>
    /// The upper bound of a range, set only for <see cref="ComparableSizeMode.Between"/> and null for every other mode.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    private readonly decimal? _maxValue;

    /// <summary>
    /// Builds the size rule, refusing a range with no upper bound rather than accepting one that could never pass.
    /// </summary>
    ///
    /// <param name="mode">Which comparison to perform.</param>
    /// <param name="value">The bound compared against, or the lower bound of a range.</param>
    /// <param name="maxValue">The upper bound, required for a range and meaningless for anything else.</param>
    ///
    /// <exception cref="ArgumentOutOfRangeException">
    /// A range was asked for without an upper bound, which would compare every size against nothing and so fail every value. Refused
    /// here rather than surfacing as a rule that rejects whatever it is given.
    /// </exception>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    public ComparableSizeValidationRule(ComparableSizeMode mode, decimal value, decimal? maxValue = null)
    {
        if (mode == ComparableSizeMode.Between && maxValue is null)
            throw new ArgumentOutOfRangeException(nameof(maxValue), "A between rule needs an upper bound.");

        _mode = mode;
        _value = value;
        _maxValue = maxValue;
    }

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

        if (!ValidationSize.TryGetComparableSize(propertyValue, out decimal size, out ValidationValueType type))
            return ValueTask.FromResult(ValidationRuleResult.Failure("validation.numeric"));

        bool isValid = _mode switch
        {
            ComparableSizeMode.Min => size >= _value,
            ComparableSizeMode.Max => size <= _value,
            ComparableSizeMode.Between => size >= _value && size <= _maxValue!.Value,
            ComparableSizeMode.Size => size == _value,
            ComparableSizeMode.GreaterThan => size > _value,
            ComparableSizeMode.GreaterThanOrEqual => size >= _value,
            ComparableSizeMode.LessThan => size < _value,
            ComparableSizeMode.LessThanOrEqual => size <= _value,
            _ => throw new InvalidOperationException($"Unsupported ComparableSizeMode value '{_mode}'.")
        };

        string messageType = ValidationSize.ToMessageType(type);

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
    private string GetMessageKey(string messageType) => _mode switch
    {
        ComparableSizeMode.Min => $"validation.min.{messageType}",
        ComparableSizeMode.Max => $"validation.max.{messageType}",
        ComparableSizeMode.Between => $"validation.between.{messageType}",
        ComparableSizeMode.Size => $"validation.size.{messageType}",
        ComparableSizeMode.GreaterThan => $"validation.greater-than.{messageType}",
        ComparableSizeMode.GreaterThanOrEqual => $"validation.greater-than-or-equal.{messageType}",
        ComparableSizeMode.LessThan => $"validation.less-than.{messageType}",
        ComparableSizeMode.LessThanOrEqual => $"validation.less-than-or-equal.{messageType}",
        _ => throw new InvalidOperationException($"Unsupported ComparableSizeMode value '{_mode}'.")
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
    private object?[] GetMessageParameters() => _mode == ComparableSizeMode.Between ? [_value, _maxValue] : [_value];
}
