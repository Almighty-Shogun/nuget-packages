using System.Linq.Expressions;
using Microsoft.Extensions.DependencyInjection;

namespace AlmightyShogun.AspNet.RequestValidation;

/// <summary>
/// Base class for a request that configures its rules in code. The rules are built once for the request type and cached, so every instance
/// of it validates against the same set.
/// </summary>
///
/// <author>Almighty-Shogun</author>
/// <since>Unreleased</since>
public abstract class ValidatableRequest<TRequest> : IValidatableRequest where TRequest : ValidatableRequest<TRequest>
{
    /// <summary>
    /// The rules collected while <see cref="Rules"/> runs. Populated on the one instance the cache happens to build from, then copied out
    /// so later mutation of this list cannot reach the cached set.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    private readonly List<IRequestValidationRule<TRequest>> _rules = [];

    /// <summary>
    /// Declares this request's rules. Called once for the request type and the result cached, so it must not branch on the values of the
    /// instance it happens to run on. Use the conditional rules, such as <c>RequiredIf</c> , for a requirement that varies per request.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    protected abstract void Rules();

    /// <summary>
    /// Begins a chain for one property, named by an expression so the field name and the value reader come from the same place.
    /// </summary>
    ///
    /// <param name="expression">
    /// Points at the property to validate. It supplies both the field name failures are reported under and the reader used to fetch the
    /// value, so the two can never disagree.
    /// </param>
    ///
    /// <returns>A builder for that property, which the caller chains rules onto.</returns>
    ///
    /// <exception cref="InvalidOperationException">
    /// <paramref name="expression"/> is not a property access, so there is no property to name the field after. Thrown as the chain is
    /// built rather than when the request is validated.
    /// </exception>
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
        var ruleCache = serviceProvider.GetRequiredService<ValidationRuleCache>();
        IReadOnlyList<IRequestValidationRule<TRequest>> rules = ruleCache.GetRequestRules(CreateFluentRules);

        ValidationBag errors = new();
        var request = (TRequest)this;

        foreach (IRequestValidationRule<TRequest> rule in rules)
        {
            if (errors.HasError(rule.FieldName)) continue;

            await rule.ValidateAsync(request, errors, serviceProvider, cancellationToken);
        }

        return errors;
    }

    /// <summary>
    /// Runs the configuration and hands back what it declared. Invoked only when the cache has nothing for this request type, so it costs
    /// one pass for the type rather than one per request.
    /// </summary>
    ///
    /// <returns>The rules this request declared, in declaration order.</returns>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    private IReadOnlyList<IRequestValidationRule<TRequest>> CreateFluentRules()
    {
        Rules();

        return [.. _rules];
    }
}
