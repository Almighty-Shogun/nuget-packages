namespace AlmightyShogun.AspNet.Validation;

/// <summary>
/// Validates date constraints for a request property.
/// </summary>
///
/// <author>Almighty-Shogun</author>
/// <since>Unreleased</since>
internal sealed class DateValidationRule<TRequest, TProperty> : IPropertyValidationRule<TRequest, TProperty> where TRequest : class
{
    private readonly DateMode _mode;

    private readonly string? _format;

    private readonly string? _target;

    private readonly DateTimeOffset? _targetDate;

    private readonly ValidationField<TRequest>? _targetField;

    /// <summary>
    /// Creates a date validation rule that accepts any valid date.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    public DateValidationRule() => _mode = DateMode.ValidDate;

    /// <summary>
    /// Creates a date validation rule that requires an exact date format.
    /// </summary>
    ///
    /// <param name="format">The required date format.</param>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    public DateValidationRule(string format)
    {
        _mode = DateMode.ExactFormat;
        _format = format;
    }

    /// <summary>
    /// Creates a date comparison rule against a literal date.
    /// </summary>
    ///
    /// <param name="mode">The date comparison mode.</param>
    /// <param name="targetDate">The literal target date.</param>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    public DateValidationRule(DateMode mode, DateTimeOffset targetDate)
    {
        _mode = mode;
        _targetDate = targetDate.ToUniversalTime();
        _target = ValidationDate.ToMessageValue(targetDate);
    }

    /// <summary>
    /// Creates a date comparison rule against another request field.
    /// </summary>
    ///
    /// <param name="mode">The date comparison mode.</param>
    /// <param name="targetField">The target field.</param>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    public DateValidationRule(DateMode mode, ValidationField<TRequest> targetField)
    {
        _mode = mode;
        _targetField = targetField;
        _target = targetField.Name;
    }

    /// <summary>
    /// Creates a date comparison rule against another request property name.
    /// </summary>
    ///
    /// <param name="mode">The date comparison mode.</param>
    /// <param name="targetPropertyName">The target property name.</param>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    public DateValidationRule(DateMode mode, string targetPropertyName)
        : this(mode, ValidationField<TRequest>.FromPropertyName(targetPropertyName)) { }

    /// <inheritdoc />
    public ValueTask<ValidationRuleResult> ValidateAsync(TRequest request, TProperty? value, string field, IServiceProvider serviceProvider, CancellationToken cancellationToken = default)
    {
        if (ValidationValue.IsEmpty(value))
            return ValueTask.FromResult(ValidationRuleResult.Success());

        if (_mode == DateMode.ExactFormat)
        {
            return ValueTask.FromResult(ValidationDate.TryGetExactDate(value, _format ?? string.Empty, out _)
                ? ValidationRuleResult.Success()
                : ValidationRuleResult.Failure("validation.date.format", _format));
        }

        if (!ValidationDate.TryGetDate(value, out DateTimeOffset date))
            return ValueTask.FromResult(ValidationRuleResult.Failure("validation.date"));

        if (_mode == DateMode.ValidDate)
            return ValueTask.FromResult(ValidationRuleResult.Success());

        if (!TryGetTargetDate(request, out DateTimeOffset targetDate))
            return ValueTask.FromResult(ValidationRuleResult.Failure(GetMessageKey(), _target));

        bool isValid = Matches(date, targetDate);

        return ValueTask.FromResult(isValid
            ? ValidationRuleResult.Success()
            : ValidationRuleResult.Failure(GetMessageKey(), _target));
    }

    /// <summary>
    /// Attempts to resolve the target date from a literal value or request field.
    /// </summary>
    ///
    /// <param name="request">The request instance.</param>
    /// <param name="targetDate">The resolved target date.</param>
    ///
    /// <returns><c>true</c> when the target date can be resolved; otherwise, <c>false</c>.</returns>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    private bool TryGetTargetDate(TRequest request, out DateTimeOffset targetDate)
    {
        if (_targetDate is not { } date)
            return ValidationDate.TryGetDate(_targetField?.GetValue(request), out targetDate);

        targetDate = date;

        return true;
    }

    /// <summary>
    /// Checks whether a date matches the configured comparison mode.
    /// </summary>
    ///
    /// <param name="date">The date to validate.</param>
    /// <param name="targetDate">The comparison target date.</param>
    ///
    /// <returns><c>true</c> when the date matches; otherwise, <c>false</c>.</returns>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    private bool Matches(DateTimeOffset date, DateTimeOffset targetDate) => _mode switch
    {
        DateMode.After => date > targetDate,
        DateMode.AfterOrEqual => date >= targetDate,
        DateMode.Before => date < targetDate,
        DateMode.BeforeOrEqual => date <= targetDate,
        DateMode.Equals => date == targetDate,
        _ => false
    };

    /// <summary>
    /// Gets the validation message key for the configured date comparison mode.
    /// </summary>
    ///
    /// <returns>The validation message key.</returns>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    private string GetMessageKey() => _mode switch
    {
        DateMode.After => "validation.after",
        DateMode.AfterOrEqual => "validation.after.or-equal",
        DateMode.Before => "validation.before",
        DateMode.BeforeOrEqual => "validation.before.or-equal",
        _ => "validation.date.equals"
    };
}
