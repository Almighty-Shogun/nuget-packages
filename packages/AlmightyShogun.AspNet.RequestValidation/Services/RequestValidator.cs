using System.Reflection;
using System.Linq.Expressions;
using System.Collections.Concurrent;

namespace AlmightyShogun.AspNet.RequestValidation;

/// <summary>
/// Runs a request's rules. Both sources are merged in the cache, so this deals in one rule set per request type rather than choosing
/// between an attribute path and a fluent one.
/// </summary>
///
/// <param name="serviceProvider">Resolves what a rule needs of its own, such as a custom rule's dependencies.</param>
/// <param name="ruleCache">The rules per request type, built on first use and kept for the life of the process.</param>
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
    private delegate Task<ValidationBag> TypedValidator(
        RequestValidator validator,
        object request,
        CancellationToken cancellationToken = default
    );

    /// <summary>
    /// The compiled entry point per request type. Static, so the delegate a type is bridged through is built once for the process
    /// rather than once per validator instance.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    private static readonly ConcurrentDictionary<Type, TypedValidator> _typedValidators = new();

    /// <summary>
    /// Validates a request against everything declared for its type, by attribute and by validator alike.
    /// </summary>
    ///
    /// <param name="request">The request object to validate.</param>
    /// <param name="cancellationToken">Cancels the work a rule does on its own, such as reading an uploaded file.</param>
    ///
    /// <returns>
    /// The failures, empty when the request passed, when it was <c>null</c> , and when its type declares no rules at all.
    /// </returns>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    public Task<ValidationBag> ValidateAsync(object? request, CancellationToken cancellationToken = default)
    {
        if (request is null || !ruleCache.HasRules(request.GetType()))
            return Task.FromResult(new ValidationBag());

        return _typedValidators.GetOrAdd(request.GetType(), CreateTypedValidator)(this, request, cancellationToken);
    }

    /// <summary>
    /// Creates a compiled delegate that calls the generic validator for a request type.
    /// </summary>
    ///
    /// <param name="requestType">The request type.</param>
    ///
    /// <returns>The compiled validator delegate.</returns>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    private static TypedValidator CreateTypedValidator(Type requestType)
    {
        ParameterExpression validatorParameter = Expression.Parameter(typeof(RequestValidator), "validator");
        ParameterExpression requestParameter = Expression.Parameter(typeof(object), "request");
        ParameterExpression cancellationTokenParameter = Expression.Parameter(typeof(CancellationToken), "cancellationToken");

        MethodInfo validateMethod = typeof(RequestValidator)
            .GetMethod(nameof(ValidateTypedAsync), BindingFlags.Instance | BindingFlags.NonPublic)!
            .MakeGenericMethod(requestType);

        MethodCallExpression call = Expression.Call(
            validatorParameter,
            validateMethod,
            Expression.Convert(requestParameter, requestType),
            cancellationTokenParameter
        );

        return Expression.Lambda<TypedValidator>(call, validatorParameter, requestParameter, cancellationTokenParameter).Compile();
    }

    /// <summary>
    /// Runs the rules for a request whose type is known, which is the generic method the cached delegate invokes.
    /// </summary>
    ///
    /// <typeparam name="TRequest">The request type, supplied by the compiled delegate for the runtime type.</typeparam>
    /// <param name="request">The typed request to validate.</param>
    /// <param name="cancellationToken">Cancels the work a rule does on its own, such as reading an uploaded file.</param>
    ///
    /// <returns>The failures, at most one per field, since a field's remaining rules are skipped once it has failed.</returns>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    private async Task<ValidationBag> ValidateTypedAsync<TRequest>(
        TRequest request,
        CancellationToken cancellationToken
    ) where TRequest : class
    {
        ValidationBag errors = new();

        foreach (IRequestValidationRule<TRequest> rule in ruleCache.GetRules<TRequest>())
        {
            if (errors.HasError(rule.FieldName)) continue;

            await rule.ValidateAsync(request, errors, serviceProvider, cancellationToken);
        }

        return errors;
    }
}
