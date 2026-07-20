namespace AlmightyShogun.AspNet.Validation;

/// <summary>
/// Validates grouped rule sets where at least one set must pass.
/// </summary>
///
/// <author>Almighty-Shogun</author>
/// <since>Unreleased</since>
internal sealed class AnyOfValidationRule<TRequest, TProperty>(IReadOnlyList<IReadOnlyList<IPropertyValidationRule<TRequest, TProperty>>> ruleSets) : IPropertyValidationRule<TRequest, TProperty> where TRequest : class
{
    /// <inheritdoc />
    public async ValueTask<ValidationRuleResult> ValidateAsync(TRequest request, TProperty? value, string field, IServiceProvider serviceProvider, CancellationToken cancellationToken = default)
    {
        if (ruleSets.Count == 0)
            return ValidationRuleResult.Success();

        foreach (IReadOnlyList<IPropertyValidationRule<TRequest, TProperty>> ruleSet in ruleSets)
        {
            bool isValid = true;

            foreach (IPropertyValidationRule<TRequest, TProperty> rule in ruleSet)
            {
                ValidationRuleResult result = await rule.ValidateAsync(request, value, field, serviceProvider, cancellationToken);

                if (result.IsValid)
                    continue;

                isValid = false;

                break;
            }

            if (isValid)
                return ValidationRuleResult.Success();
        }

        return ValidationRuleResult.Failure("validation.any-of");
    }
}
