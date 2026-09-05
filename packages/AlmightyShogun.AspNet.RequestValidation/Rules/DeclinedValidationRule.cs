namespace AlmightyShogun.AspNet.RequestValidation;

/// <summary>
/// Requires a value that reads as a no. Like its opposite it does not skip an empty value, since absence is the case it exists to catch.
/// </summary>
///
/// <typeparam name="TRequest">The request type the rule is declared on, though only the bound value decides the outcome.</typeparam>
/// <typeparam name="TProperty">The bound property's type, read as an object rather than constrained here.</typeparam>
///
/// <author>Almighty-Shogun</author>
/// <since>4.0.0</since>
internal sealed class DeclinedValidationRule<TRequest, TProperty> : IPropertyValidationRule<TRequest, TProperty> where TRequest : class
{
    /// <inheritdoc />
    public ValueTask<ValidationRuleResult> ValidateAsync(
        TRequest request,
        TProperty? value,
        string field,
        IServiceProvider serviceProvider,
        CancellationToken cancellationToken = default
    ) => ValueTask.FromResult(ValidationValue.IsDeclined(value)
        ? ValidationRuleResult.Success()
        : ValidationRuleResult.Failure("validation.declined"));
}
