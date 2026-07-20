using System.Linq.Expressions;

namespace AlmightyShogun.AspNet.Validation;

/// <summary>
/// Validates conditional state constraints for a request property.
/// </summary>
///
/// <author>Almighty-Shogun</author>
/// <since>Unreleased</since>
internal sealed class ConditionalStateValidationRule<TRequest, TProperty, TCompare>(ConditionalStateTargetMode targetMode, ConditionalStateMode stateMode, Expression<Func<TRequest, TCompare>> compareExpression) : IPropertyValidationRule<TRequest, TProperty> where TRequest : class
{
    private readonly ValidationField<TRequest> _field = ValidationField<TRequest>.From(compareExpression);

    /// <inheritdoc />
    public ValueTask<ValidationRuleResult> ValidateAsync(TRequest request, TProperty? value, string field, IServiceProvider serviceProvider, CancellationToken cancellationToken = default)
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
    /// Gets the validation message key for the configured conditional state mode.
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
        _ => "validation.prohibited.if-declined"
    };
}
