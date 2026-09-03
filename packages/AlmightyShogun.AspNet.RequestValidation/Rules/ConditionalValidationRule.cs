using System.Linq.Expressions;

namespace AlmightyShogun.AspNet.RequestValidation;

/// <summary>
/// Applies a presence requirement to this field only when a controlling field equals one of the configured values.
/// </summary>
///
/// <author>Almighty-Shogun</author>
/// <since>Unreleased</since>
internal sealed class ConditionalValidationRule<TRequest, TProperty, TCompare>(
    ConditionalTargetMode targetMode,
    ConditionMode conditionMode,
    Expression<Func<TRequest, TCompare>> compareExpression,
    IReadOnlyList<TCompare?> values
) : IPropertyValidationRule<TRequest, TProperty> where TRequest : class
{
    /// <inheritdoc />
    public ValidationRulePriority Priority => ValidationRulePriority.Required;

    /// <summary>
    /// The condition deciding whether this rule applies, built once from the controlling field and the values it is matched against.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    private readonly ValidationFieldCondition<TRequest, TCompare> _condition = new(compareExpression, values);

    /// <inheritdoc />
    public ValueTask<ValidationRuleResult> ValidateAsync(
        TRequest request,
        TProperty? value,
        string field,
        IServiceProvider serviceProvider,
        CancellationToken cancellationToken = default
    )
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
    /// Checks whether the target value satisfies the configured conditional target mode, once the condition has already matched.
    /// </summary>
    ///
    /// <param name="value">The value read from the property this rule guards.</param>
    ///
    /// <returns>
    /// <c>true</c> when the value satisfies the mode; otherwise, <c>false</c> . An absent value is not skipped: once the condition
    /// matches, an accepted or declined requirement is a requirement, and a field left out is exactly the case it has to catch. That
    /// matches the unconditional accepted and declined rules, which do not skip an empty value either.
    /// </returns>
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
        _ => throw new InvalidOperationException($"Unsupported ConditionalTargetMode value '{targetMode}'.")
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
        (ConditionalTargetMode.Accepted, ConditionMode.If) => "validation.accepted.if",
        (ConditionalTargetMode.Declined, ConditionMode.If) => "validation.declined.if",
        _ => throw new InvalidOperationException(
            $"Unsupported ConditionalTargetMode and ConditionMode pairing: ({targetMode}, {conditionMode})."
        )
    };
}
