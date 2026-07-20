namespace AlmightyShogun.AspNet.Validation;

/// <summary>
/// Validates declined constraints for a request property.
/// </summary>
///
/// <author>Almighty-Shogun</author>
/// <since>Unreleased</since>
internal sealed class DeclinedValidationRule<TRequest, TProperty> : IPropertyValidationRule<TRequest, TProperty> where TRequest : class
{
    /// <inheritdoc />
    public ValueTask<ValidationRuleResult> ValidateAsync(TRequest request, TProperty? value, string field, IServiceProvider serviceProvider, CancellationToken cancellationToken = default)
        => ValueTask.FromResult(ValidationValue.IsDeclined(value)
            ? ValidationRuleResult.Success()
            : ValidationRuleResult.Failure("validation.declined"));
}
