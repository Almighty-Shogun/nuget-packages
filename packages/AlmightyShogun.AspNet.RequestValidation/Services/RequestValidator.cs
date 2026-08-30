using System.Reflection;
using System.Linq.Expressions;
using System.Collections.Concurrent;

namespace AlmightyShogun.AspNet.RequestValidation;

/// <summary>
/// Runs a request's rules, choosing between the fluent rules a request declares itself and the rules built from its attributes. Both paths
/// read from the same cache, so neither pays reflection per request.
/// </summary>
///
/// <author>Almighty-Shogun</author>
/// <since>Unreleased</since>
internal sealed class RequestValidator(IServiceProvider serviceProvider, ValidationRuleCache ruleCache)
{
    /// <summary>
    /// The compiled entry point for one request type. Caching a delegate rather than the rules avoids constructing a generic method on
    /// every request when the type is only known at runtime.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    private delegate Task<ValidationBag> AttributeValidator(
        RequestValidator validator,
        object request,
        CancellationToken cancellationToken
    );

    /// <summary>
    /// The compiled entry point per request type. Static, so the delegate a type is bridged through is built once for the process
    /// rather than once per validator instance.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    private static readonly ConcurrentDictionary<Type, AttributeValidator> AttributeValidators = new();

    /// <summary>
    /// Validates a request, preferring the rules it declares itself and falling back to the ones its attributes declare.
    /// </summary>
    ///
    /// <param name="request">The request object to validate.</param>
    /// <param name="cancellationToken">Cancels the work a rule does on its own, such as reading an uploaded file.</param>
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
    /// Reports whether a type declares any attribute rules, so a request with none skips rule building rather than building an empty set.
    /// </summary>
    ///
    /// <param name="request">The request object to inspect.</param>
    ///
    /// <returns>
    /// <c>true</c> when the request type is a class carrying at least one validation attribute; otherwise, <c>false</c>.
    /// </returns>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    private bool HasAttributeRules(object request) => ruleCache.HasAttributeRules(request.GetType());

    /// <summary>
    /// Validates a request with cached attribute rules using a compiled type-specific delegate.
    /// </summary>
    ///
    /// <param name="request">The request object to validate.</param>
    /// <param name="cancellationToken">Cancels the work a rule does on its own, such as reading an uploaded file.</param>
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
            cancellationTokenParameter
        );

        return Expression.Lambda<AttributeValidator>(call, validatorParameter, requestParameter, cancellationTokenParameter).Compile();
    }

    /// <summary>
    /// Runs the attribute rules for a request whose type is known, which is the generic method the cached delegate invokes.
    /// </summary>
    ///
    /// <param name="request">The typed request to validate.</param>
    /// <param name="cancellationToken">Cancels the work a rule does on its own, such as reading an uploaded file.</param>
    ///
    /// <returns>The validation error bag.</returns>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    private async Task<ValidationBag> ValidateTypedAttributeRulesAsync<TRequest>(
        TRequest request,
        CancellationToken cancellationToken
    ) where TRequest : class
    {
        ValidationBag errors = new();

        foreach (IRequestValidationRule<TRequest> rule in ruleCache.GetAttributeRules<TRequest>())
        {
            if (errors.HasError(rule.FieldName)) continue;

            await rule.ValidateAsync(request, errors, serviceProvider, cancellationToken);
        }

        return errors;
    }
}
