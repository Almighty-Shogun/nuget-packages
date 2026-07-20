namespace AlmightyShogun.AspNet.Validation;

/// <summary>
/// Validates distinct constraints for a request property.
/// </summary>
///
/// <author>Almighty-Shogun</author>
/// <since>Unreleased</since>
internal sealed class DistinctValidationRule<TRequest, TProperty> : IPropertyValidationRule<TRequest, TProperty> where TRequest : class
{
    /// <inheritdoc />
    public ValueTask<ValidationRuleResult> ValidateAsync(TRequest request, TProperty? value, string field, IServiceProvider serviceProvider, CancellationToken cancellationToken = default)
    {
        if (ValidationValue.IsEmpty(value))
            return ValueTask.FromResult(ValidationRuleResult.Success());

        if (!ValidationCollection.TryGetValues(value, out IReadOnlyList<object?> values))
            return ValueTask.FromResult(ValidationRuleResult.Failure("validation.distinct"));

        HashSet<object?> seen = [];

        return ValueTask.FromResult(values.All(seen.Add)
            ? ValidationRuleResult.Success()
            : ValidationRuleResult.Failure("validation.distinct"));
    }
}
