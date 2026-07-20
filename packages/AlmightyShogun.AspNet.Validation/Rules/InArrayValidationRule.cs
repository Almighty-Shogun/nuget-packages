namespace AlmightyShogun.AspNet.Validation;

/// <summary>
/// Validates in array constraints for a request property.
/// </summary>
///
/// <author>Almighty-Shogun</author>
/// <since>Unreleased</since>
internal sealed class InArrayValidationRule<TRequest, TProperty>(ValidationField<TRequest> compareField) : IPropertyValidationRule<TRequest, TProperty> where TRequest : class
{
    /// <inheritdoc />
    public ValueTask<ValidationRuleResult> ValidateAsync(TRequest request, TProperty? value, string field, IServiceProvider serviceProvider, CancellationToken cancellationToken = default)
    {
        if (ValidationValue.IsEmpty(value))
            return ValueTask.FromResult(ValidationRuleResult.Success());

        if (!ValidationCollection.TryGetValues(compareField.GetValue(request), out IReadOnlyList<object?> values))
            return ValueTask.FromResult(ValidationRuleResult.Failure("validation.in.array", compareField.Name));

        bool isValid = values.Contains(value);

        return ValueTask.FromResult(isValid
            ? ValidationRuleResult.Success()
            : ValidationRuleResult.Failure("validation.in.array", compareField.Name));
    }
}
