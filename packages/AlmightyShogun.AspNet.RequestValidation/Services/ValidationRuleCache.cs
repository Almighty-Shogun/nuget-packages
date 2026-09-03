using System.Collections.Concurrent;

namespace AlmightyShogun.AspNet.RequestValidation;

/// <summary>
/// Holds the rules for each request type for the life of the process, so reflection, rule construction, and merging happen once rather
/// than per request.
/// </summary>
///
/// <param name="validatorRegistry">
/// The validators found at startup, asked for a request type's fluent rules when the cache first builds that type's set.
/// </param>
///
/// <author>Almighty-Shogun</author>
/// <since>Unreleased</since>
internal sealed class ValidationRuleCache(ValidatorRegistry validatorRegistry)
{
    /// <summary>
    /// The merged attribute and fluent rules per request type, boxed because the value is generic in the type it is keyed by.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    private readonly ConcurrentDictionary<Type, object> _requestRules = new();

    /// <summary>
    /// Whether a type declares any attribute rules, cached so a request carrying none skips the reflection every time, not just once.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    private readonly ConcurrentDictionary<Type, bool> _hasAttributeRules = new();

    /// <summary>
    /// Reports whether a type has any rules at all, from either source, so a request with none skips rule building entirely.
    /// </summary>
    ///
    /// <param name="requestType">The request type.</param>
    ///
    /// <returns>
    /// <c>true</c> when the request type is a class that either carries a validation attribute or has a validator; otherwise,
    /// <c>false</c>. A struct reports <c>false</c> whatever its properties declare, since only reference types are validated.
    /// </returns>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    public bool HasRules(Type requestType)
        => requestType.IsClass
           && (validatorRegistry.HasValidator(requestType) || _hasAttributeRules.GetOrAdd(requestType, AttributeRuleFactory.HasRules));

    /// <summary>
    /// Gets the rules for a request type, building them on first use and keeping them thereafter.
    /// </summary>
    ///
    /// <typeparam name="TRequest">The request type whose rules are built and then cached against it.</typeparam>
    ///
    /// <returns>
    /// The merged rules: what the type's attributes declare followed by what its validator declares, combined per field and with
    /// duplicate identities removed, so a constraint declared both ways is checked once.
    /// </returns>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    public IReadOnlyList<IRequestValidationRule<TRequest>> GetRules<TRequest>() where TRequest : class
        => (IReadOnlyList<IRequestValidationRule<TRequest>>)_requestRules.GetOrAdd(
            typeof(TRequest),
            _ => ValidationRuleMerger.Merge(
                AttributeRuleFactory.CreateRules<TRequest>().Concat(validatorRegistry.BuildRules<TRequest>())
            )
        );
}
