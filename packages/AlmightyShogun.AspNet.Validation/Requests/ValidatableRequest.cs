using System.Linq.Expressions;
using Microsoft.Extensions.DependencyInjection;

namespace AlmightyShogun.AspNet.Validation;

/// <summary>
/// Base request type for defining validation rules inside a request class.
/// </summary>
///
/// <author>Almighty-Shogun</author>
/// <since>Unreleased</since>
public abstract class ValidatableRequest<TRequest> : IValidatableRequest where TRequest : ValidatableRequest<TRequest>
{
    private bool _rulesConfigured;

    private readonly List<IRequestValidationRule<TRequest>> _rules = [];

    /// <summary>
    /// Configures validation rules for the request.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    protected abstract void Rules();

    /// <summary>
    /// Starts a validation rule chain for a request property.
    /// </summary>
    ///
    /// <param name="expression">The property expression to validate.</param>
    ///
    /// <returns>The rule builder for the selected property.</returns>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    protected RuleBuilder<TRequest, TProperty> RuleFor<TProperty>(Expression<Func<TRequest, TProperty>> expression)
    {
        PropertyRule<TRequest, TProperty> propertyRule = new(expression);

        _rules.Add(propertyRule);

        return new RuleBuilder<TRequest, TProperty>(propertyRule);
    }

    /// <inheritdoc />
    async Task<ValidationBag> IValidatableRequest.ValidateAsync(IServiceProvider serviceProvider, CancellationToken cancellationToken)
    {
        ValidationRuleCache ruleCache = serviceProvider.GetRequiredService<ValidationRuleCache>();
        IReadOnlyList<IRequestValidationRule<TRequest>> rules = ruleCache.GetRequestRules(CreateFluentRules);

        ValidationBag errors = new();
        var request = (TRequest)this;

        foreach (IRequestValidationRule<TRequest> rule in rules)
        {
            if (errors.HasError(rule.FieldName))
                continue;

            await rule.ValidateAsync(request, errors, serviceProvider, cancellationToken);
        }

        return errors;
    }

    /// <summary>
    /// Creates and caches fluent validation rules for the request instance.
    /// </summary>
    ///
    /// <returns>The configured request validation rules.</returns>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    private IReadOnlyList<IRequestValidationRule<TRequest>> CreateFluentRules()
    {
        if (_rulesConfigured)
            return _rules.ToArray();

        Rules();
        _rulesConfigured = true;

        return _rules.ToArray();
    }
}
