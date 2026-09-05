using System.Linq.Expressions;

namespace AlmightyShogun.AspNet.RequestValidation;

/// <summary>
/// Applies a presence requirement to this field only when a controlling field reads as accepted or declined. An empty value is not
/// skipped: once the controlling field matches, emptiness is exactly what the requirement tests.
/// </summary>
///
/// <typeparam name="TRequest">The request type the rule is declared on and reads the controlling field from.</typeparam>
/// <typeparam name="TProperty">The bound property's type, read as an object and tested only for emptiness.</typeparam>
/// <typeparam name="TCompare">The controlling field's type, whose value is read as an object for the accepted or declined test.</typeparam>
/// <param name="targetMode">Whether the matched condition requires this field or prohibits it.</param>
/// <param name="stateMode">Which reading of the controlling field, accepted or declined, makes the requirement apply.</param>
/// <param name="compareExpression">Points at the controlling field, resolved once when the rule is built.</param>
///
/// <exception cref="ArgumentOutOfRangeException">
/// <paramref name="compareExpression"/> is not a property read directly off the request. Thrown as the rule is built rather than when a
/// request arrives.
/// </exception>
///
/// <author>Almighty-Shogun</author>
/// <since>Unreleased</since>
internal sealed class ConditionalStateValidationRule<TRequest, TProperty, TCompare>(
    ConditionalStateTargetMode targetMode,
    ConditionalStateMode stateMode,
    Expression<Func<TRequest, TCompare>> compareExpression
) : IPropertyValidationRule<TRequest, TProperty> where TRequest : class
{
    /// <inheritdoc />
    public ValidationRulePriority Priority => ValidationRulePriority.Required;

    /// <summary>
    /// The controlling field this rule watches, resolved once when the rule is built so each request only reads its value.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    private readonly ValidationField<TRequest> _field = ValidationField<TRequest>.From(compareExpression);

    /// <inheritdoc />
    public ValueTask<ValidationRuleResult> ValidateAsync(
        TRequest request,
        TProperty? value,
        string field,
        IServiceProvider serviceProvider,
        CancellationToken cancellationToken = default
    )
    {
        bool shouldValidate = stateMode == ConditionalStateMode.Accepted
            ? ValidationValue.IsAccepted(_field.GetValue(request))
            : ValidationValue.IsDeclined(_field.GetValue(request));

        if (!shouldValidate)
            return ValueTask.FromResult(ValidationRuleResult.Success());

        bool isValid = targetMode == ConditionalStateTargetMode.Required
            ? !ValidationValue.IsEmpty(value)
            : ValidationValue.IsEmpty(value);

        return ValueTask.FromResult(isValid
            ? ValidationRuleResult.Success()
            : ValidationRuleResult.Failure(GetMessageKey(), _field.Name));
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
    private string GetMessageKey() => (targetMode, stateMode) switch
    {
        (ConditionalStateTargetMode.Required, ConditionalStateMode.Accepted) => "validation.required.if-accepted",
        (ConditionalStateTargetMode.Required, ConditionalStateMode.Declined) => "validation.required.if-declined",
        (ConditionalStateTargetMode.Prohibited, ConditionalStateMode.Accepted) => "validation.prohibited.if-accepted",
        (ConditionalStateTargetMode.Prohibited, ConditionalStateMode.Declined) => "validation.prohibited.if-declined",
        _ => throw new InvalidOperationException(
            $"Unsupported ConditionalStateTargetMode and ConditionalStateMode pairing: ({targetMode}, {stateMode})."
        )
    };
}
