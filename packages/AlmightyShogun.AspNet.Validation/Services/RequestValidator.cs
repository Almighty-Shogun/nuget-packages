using System.Reflection;
using System.Linq.Expressions;
using System.Collections.Concurrent;

namespace AlmightyShogun.AspNet.Validation;

/// <summary>
/// Validates request instances using cached validation rules.
/// </summary>
///
/// <author>Almighty-Shogun</author>
/// <since>Unreleased</since>
internal sealed class RequestValidator(IServiceProvider serviceProvider, ValidationRuleCache ruleCache)
{
    /// <summary>
    /// Defines a cached delegate that validates request attributes.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    private delegate Task<ValidationBag> AttributeValidator(RequestValidator validator, object request, CancellationToken cancellationToken);

    private static readonly ConcurrentDictionary<Type, AttributeValidator> AttributeValidators = new();

    /// <summary>
    /// Validates a request object using validatable request rules or attribute rules.
    /// </summary>
    ///
    /// <param name="request">The request object to validate.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    ///
    /// <returns>The validation error bag.</returns>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    public async Task<ValidationBag> ValidateAsync(object? request, CancellationToken cancellationToken = default) => request switch
    {
        null => new ValidationBag(),
        IValidatableRequest validatableRequest => await validatableRequest.ValidateAsync(serviceProvider, cancellationToken),
        not null when HasAttributeRules(request) => await ValidateAttributeRulesAsync(request, cancellationToken),
        _ => new ValidationBag()
    };

    /// <summary>
    /// Checks whether a request type has attribute validation rules.
    /// </summary>
    ///
    /// <param name="request">The request object to inspect.</param>
    ///
    /// <returns><c>true</c> when the request type has attribute rules; otherwise, <c>false</c>.</returns>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    private bool HasAttributeRules(object request) => ruleCache.HasAttributeRules(request.GetType());

    /// <summary>
    /// Validates a request with cached attribute rules using a compiled type-specific delegate.
    /// </summary>
    ///
    /// <param name="request">The request object to validate.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    ///
    /// <returns>The validation error bag.</returns>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    private Task<ValidationBag> ValidateAttributeRulesAsync(object request, CancellationToken cancellationToken)
        => AttributeValidators.GetOrAdd(request.GetType(), CreateAttributeValidator)(this, request, cancellationToken);

    /// <summary>
    /// Creates a compiled delegate that calls the generic attribute validator for a request type.
    /// </summary>
    ///
    /// <param name="requestType">The request type.</param>
    ///
    /// <returns>The compiled attribute validator delegate.</returns>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    private static AttributeValidator CreateAttributeValidator(Type requestType)
    {
        ParameterExpression validatorParameter = Expression.Parameter(typeof(RequestValidator), "validator");
        ParameterExpression requestParameter = Expression.Parameter(typeof(object), "request");
        ParameterExpression cancellationTokenParameter = Expression.Parameter(typeof(CancellationToken), "cancellationToken");

        MethodInfo validateMethod = typeof(RequestValidator)
            .GetMethod(nameof(ValidateTypedAttributeRulesAsync), BindingFlags.Instance | BindingFlags.NonPublic)!
            .MakeGenericMethod(requestType);

        MethodCallExpression call = Expression.Call(
            validatorParameter,
            validateMethod,
            Expression.Convert(requestParameter, requestType),
            cancellationTokenParameter);

        return Expression
            .Lambda<AttributeValidator>(call, validatorParameter, requestParameter, cancellationTokenParameter)
            .Compile();
    }

    /// <summary>
    /// Validates a typed request with cached attribute rules.
    /// </summary>
    ///
    /// <param name="request">The typed request to validate.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    ///
    /// <returns>The validation error bag.</returns>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    private async Task<ValidationBag> ValidateTypedAttributeRulesAsync<TRequest>(TRequest request, CancellationToken cancellationToken) where TRequest : class
    {
        ValidationBag errors = new();

        foreach (IRequestValidationRule<TRequest> rule in ruleCache.GetAttributeRules<TRequest>())
        {
            if (errors.HasError(rule.FieldName))
                continue;

            await rule.ValidateAsync(request, errors, serviceProvider, cancellationToken);
        }

        return errors;
    }
}
