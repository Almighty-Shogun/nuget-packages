namespace AlmightyShogun.AspNet.RequestValidation;

/// <summary>
/// Checks that a value is a date, matches an exact format, or orders correctly against a literal date or another field.
/// </summary>
///
/// <author>Almighty-Shogun</author>
/// <since>Unreleased</since>
internal sealed class DateValidationRule<TRequest, TProperty> : IPropertyValidationRule<TRequest, TProperty> where TRequest : class
{
    /// <summary>
    /// Which date check this rule performs, set by whichever constructor was used rather than passed in on its own.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    private readonly DateMode _mode;

    /// <summary>
    /// The exact format a value must parse under, set only for the exact-format check and left null for every other mode. Non-empty
    /// whenever it is set, since the constructor that sets it refuses a blank one.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    private readonly string? _format;

    /// <summary>
    /// The comparison target as it was written, kept for the failure message so a client is told what the date was measured against.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    private readonly string? _target;

    /// <summary>
    /// The comparison target as a fixed moment, set when the target was a literal rather than another field on the request.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    private readonly DateTimeOffset? _targetDate;

    /// <summary>
    /// The field the comparison target is read from, set when the target names another property rather than a literal date.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    private readonly ValidationField<TRequest>? _targetField;

    /// <summary>
    /// Builds the rule that only requires the value to be a date at all, with no format or ordering constraint.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    public DateValidationRule() => _mode = DateMode.ValidDate;

    /// <summary>
    /// Builds the rule that requires one exact format, so a date written any other way fails even though it names a real instant.
    /// </summary>
    ///
    /// <param name="format">The required date format.</param>
    ///
    /// <exception cref="ArgumentException">
    /// <paramref name="format"/> is empty or whitespace, which no value can ever parse under, so the rule would reject every value it was
    /// given. Refused here rather than surfacing as a field that cannot be filled in.
    /// </exception>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    public DateValidationRule(string format)
    {
        if (string.IsNullOrWhiteSpace(format))
            throw new ArgumentException("A date format rule needs a format.", nameof(format));

        _format = format;
        _mode = DateMode.ExactFormat;
    }

    /// <summary>
    /// Builds an ordering rule against a fixed date decided when the rule was written.
    /// </summary>
    ///
    /// <param name="mode">Which ordering the value must satisfy against the target.</param>
    /// <param name="targetDate">The literal target date.</param>
    ///
    /// <exception cref="ArgumentOutOfRangeException">
    /// <paramref name="mode"/> is not an ordering, so pairing it with a target is meaningless.
    /// </exception>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    public DateValidationRule(DateMode mode, DateTimeOffset targetDate)
    {
        _mode = RequireOrderingMode(mode);
        _targetDate = targetDate.ToUniversalTime();
        _target = ValidationDate.ToMessageValue(targetDate);
    }

    /// <summary>
    /// Builds an ordering rule against another property, named by expression, so the bound is whatever that field holds per request.
    /// </summary>
    ///
    /// <param name="mode">Which ordering the value must satisfy against the target.</param>
    /// <param name="targetField">The target field.</param>
    ///
    /// <exception cref="ArgumentOutOfRangeException">
    /// <paramref name="mode"/> is not an ordering, so pairing it with a target field is meaningless.
    /// </exception>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    public DateValidationRule(DateMode mode, ValidationField<TRequest> targetField)
    {
        _mode = RequireOrderingMode(mode);
        _targetField = targetField;
        _target = targetField.Name;
    }

    /// <summary>
    /// Builds an ordering rule against another property named as a string, which is the attribute spelling.
    /// </summary>
    ///
    /// <param name="mode">Which ordering the value must satisfy against the target.</param>
    /// <param name="targetPropertyName">The target property name.</param>
    ///
    /// <exception cref="ArgumentOutOfRangeException">
    /// <paramref name="mode"/> is not an ordering. A <paramref name="targetPropertyName"/> naming no property on the request
    /// type raises <see cref="InvalidOperationException"/> instead, from the field lookup, before this check is reached.
    /// </exception>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    public DateValidationRule(DateMode mode, string targetPropertyName)
        : this(mode, ValidationField<TRequest>.FromPropertyName(targetPropertyName)) { }

    /// <inheritdoc />
    public ValueTask<ValidationRuleResult> ValidateAsync(
        TRequest request,
        TProperty? value,
        string field,
        IServiceProvider serviceProvider,
        CancellationToken cancellationToken = default
    )
    {
        if (ValidationValue.IsEmpty(value))
            return ValueTask.FromResult(ValidationRuleResult.Success());

        if (_mode is DateMode.ExactFormat)
            return ValueTask.FromResult(ValidationDate.TryGetExactDate(value, _format!, out _)
                ? ValidationRuleResult.Success()
                : ValidationRuleResult.Failure("validation.date.format", _format));

        if (!ValidationDate.TryGetDate(value, out DateTimeOffset date))
            return ValueTask.FromResult(ValidationRuleResult.Failure("validation.date"));

        if (_mode is DateMode.ValidDate)
            return ValueTask.FromResult(ValidationRuleResult.Success());

        if (!TryGetTargetDate(request, out DateTimeOffset targetDate))
            return ValueTask.FromResult(ValidationRuleResult.Failure(GetMessageKey(), _target));

        bool isValid = Matches(date, targetDate);

        return ValueTask.FromResult(isValid ? ValidationRuleResult.Success() : ValidationRuleResult.Failure(GetMessageKey(), _target));
    }

    /// <summary>
    /// Attempts to resolve the target date from a literal value or request field.
    /// </summary>
    ///
    /// <param name="request">The request being validated, so a rule can read another field as well as its own.</param>
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
    /// <param name="date">The value already read as a date, compared against the target this rule was built with.</param>
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
        _ => throw new InvalidOperationException($"Unsupported DateMode value '{_mode}'.")
    };

    /// <summary>
    /// Accepts only the modes that compare against a target, so a rule cannot be built pairing a target with a mode that ignores it.
    /// </summary>
    ///
    /// <param name="mode">The mode a comparison constructor was handed.</param>
    ///
    /// <returns>The mode, once it is known to be an ordering.</returns>
    ///
    /// <exception cref="ArgumentOutOfRangeException">
    /// The mode is <see cref="DateMode.ValidDate"/> or <see cref="DateMode.ExactFormat"/> , neither of which reads a target. Refused
    /// here rather than accepted and then ignored, since a comparison rule holding a mode that never compares would report a failure key
    /// for a check it never made.
    /// </exception>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    private static DateMode RequireOrderingMode(DateMode mode) => mode switch
    {
        DateMode.After or DateMode.AfterOrEqual or DateMode.Before or DateMode.BeforeOrEqual or DateMode.Equals => mode,
        _ => throw new ArgumentOutOfRangeException(nameof(mode), mode, "A date comparison rule needs an ordering mode.")
    };

    /// <summary>
    /// Maps the configured mode onto the message key its failure reports, so one rule class serves every spelling of its family without
    /// each needing a class of its own.
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
        DateMode.Equals => "validation.date.equals",
        _ => throw new InvalidOperationException($"Unsupported DateMode value '{_mode}'.")
    };
}
