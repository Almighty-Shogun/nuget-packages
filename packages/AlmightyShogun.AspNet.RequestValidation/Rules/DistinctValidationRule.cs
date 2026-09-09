namespace AlmightyShogun.AspNet.RequestValidation;

/// <summary>
/// Requires every entry of a collection to be unique.
/// An absent or empty value passes without being checked, so the rule never implies the field is required.
/// </summary>
///
/// <typeparam name="TRequest">The request type the rule is declared on, though only the bound value decides the outcome.</typeparam>
/// <typeparam name="TProperty">The bound property's type; a non-empty value that is no readable collection fails.</typeparam>
///
/// <author>Almighty-Shogun</author>
/// <since>4.0.0</since>
internal sealed class DistinctValidationRule<TRequest, TProperty> : IPropertyValidationRule<TRequest, TProperty> where TRequest : class
{
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

        if (!ValidationCollection.TryGetValues(value, out IReadOnlyList<object?> values))
            return ValueTask.FromResult(ValidationRuleResult.Failure("validation.distinct"));

        HashSet<object?> seen = [];

        return ValueTask.FromResult(values.All(seen.Add)
            ? ValidationRuleResult.Success()
            : ValidationRuleResult.Failure("validation.distinct"));
    }
}
