namespace AlmightyShogun.AspNet.Validation;

/// <summary>
/// Validates named conditional constraints for a request property.
/// </summary>
///
/// <author>Almighty-Shogun</author>
/// <since>Unreleased</since>
internal sealed class NamedConditionalValidationRule<TRequest, TProperty>(ConditionalTargetMode targetMode, ConditionMode conditionMode, string comparePropertyName, IReadOnlyList<object?> values) : IPropertyValidationRule<TRequest, TProperty> where TRequest : class
{
    private readonly NamedValidationFieldCondition<TRequest> _condition = new(comparePropertyName, values);

    /// <inheritdoc />
    public ValueTask<ValidationRuleResult> ValidateAsync(TRequest request, TProperty? value, string field, IServiceProvider serviceProvider, CancellationToken cancellationToken = default)
    {
        bool conditionMatches = _condition.Matches(request);
        bool shouldValidate = conditionMode == ConditionMode.If ? conditionMatches : !conditionMatches;

        if (!shouldValidate)
            return ValueTask.FromResult(ValidationRuleResult.Success());

        return ValueTask.FromResult(IsValid(value)
            ? ValidationRuleResult.Success()
            : ValidationRuleResult.Failure(GetMessageKey(), _condition.FieldName, _condition.ValuesText));
    }

    /// <summary>
    /// Checks whether the target value satisfies the configured conditional target mode.
    /// </summary>
    ///
    /// <param name="value">The target value.</param>
    ///
    /// <returns><c>true</c> when the target value is valid; otherwise, <c>false</c>.</returns>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    private bool IsValid(TProperty? value) => targetMode switch
    {
        ConditionalTargetMode.Missing => value is null,
        ConditionalTargetMode.Required => !ValidationValue.IsEmpty(value),
        ConditionalTargetMode.Present => ValidationValue.IsPresent(value),
        ConditionalTargetMode.Prohibited => ValidationValue.IsEmpty(value),
        ConditionalTargetMode.Accepted => ValidationValue.IsAccepted(value),
        ConditionalTargetMode.Declined => ValidationValue.IsDeclined(value),
        _ => false
    };

    /// <summary>
    /// Gets the validation message key for the configured conditional mode.
    /// </summary>
    ///
    /// <returns>The validation message key.</returns>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    private string GetMessageKey() => (targetMode, conditionMode) switch
    {
        (ConditionalTargetMode.Required, ConditionMode.If) => "validation.required.if",
        (ConditionalTargetMode.Required, ConditionMode.Unless) => "validation.required.unless",
        (ConditionalTargetMode.Present, ConditionMode.If) => "validation.present.if",
        (ConditionalTargetMode.Present, ConditionMode.Unless) => "validation.present.unless",
        (ConditionalTargetMode.Missing, ConditionMode.If) => "validation.missing.if",
        (ConditionalTargetMode.Missing, ConditionMode.Unless) => "validation.missing.unless",
        (ConditionalTargetMode.Prohibited, ConditionMode.If) => "validation.prohibited.if",
        (ConditionalTargetMode.Prohibited, ConditionMode.Unless) => "validation.prohibited.unless",
        (ConditionalTargetMode.Accepted, _) => "validation.accepted.if",
        _ => "validation.declined.if"
    };
}
