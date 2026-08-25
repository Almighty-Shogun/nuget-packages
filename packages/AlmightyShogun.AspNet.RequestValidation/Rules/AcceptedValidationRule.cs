namespace AlmightyShogun.AspNet.RequestValidation;

/// <summary>
/// Requires a value that reads as a yes. It does not skip an empty value, because a checkbox left unticked is exactly the case it must
/// catch.
/// </summary>
///
/// <author>Almighty-Shogun</author>
/// <since>Unreleased</since>
internal sealed class AcceptedValidationRule<TRequest, TProperty> : IPropertyValidationRule<TRequest, TProperty> where TRequest : class
{
    /// <inheritdoc />
    public ValueTask<ValidationRuleResult> ValidateAsync(
        TRequest request,
        TProperty? value,
        string field,
        IServiceProvider serviceProvider,
        CancellationToken cancellationToken = default
    ) => ValueTask.FromResult(ValidationValue.IsAccepted(value)
        ? ValidationRuleResult.Success()
        : ValidationRuleResult.Failure("validation.accepted"));
}
