namespace AlmightyShogun.AspNet.RequestValidation;

/// <summary>
/// Passes when any one of the grouped rule sets passes. Failures from the losing sets are discarded and the field reports a single
/// <c>validation.any-of</c> failure instead, naming no particular alternative. Emptiness is not tested here at all: each grouped rule
/// applies whatever handling of an empty value it has of its own.
/// </summary>
///
/// <typeparam name="TRequest">The request type the rule is declared on and hands to every grouped rule.</typeparam>
/// <typeparam name="TProperty">The bound property's type, shared with the grouped rules, which are handed the same value.</typeparam>
/// <param name="ruleSets">The alternatives, each a set whose rules must all pass. An empty outer list passes.</param>
///
/// <author>Almighty-Shogun</author>
/// <since>Unreleased</since>
internal sealed class AnyOfValidationRule<TRequest, TProperty>(
    IReadOnlyList<IReadOnlyList<IPropertyValidationRule<TRequest, TProperty>>> ruleSets
) : IPropertyValidationRule<TRequest, TProperty> where TRequest : class
{
    /// <inheritdoc />
    public async ValueTask<ValidationRuleResult> ValidateAsync(
        TRequest request,
        TProperty? value,
        string field,
        IServiceProvider serviceProvider,
        CancellationToken cancellationToken = default
    )
    {
        if (ruleSets.Count == 0)
            return ValidationRuleResult.Success();

        foreach (IReadOnlyList<IPropertyValidationRule<TRequest, TProperty>> ruleSet in ruleSets)
        {
            var isValid = true;

            foreach (IPropertyValidationRule<TRequest, TProperty> rule in ruleSet)
            {
                ValidationRuleResult result = await rule.ValidateAsync(request, value, field, serviceProvider, cancellationToken);

                if (result.IsValid) continue;

                isValid = false;

                break;
            }

            if (isValid)
                return ValidationRuleResult.Success();
        }

        return ValidationRuleResult.Failure("validation.any-of");
    }
}
