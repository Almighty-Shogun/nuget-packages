using System.Collections.Concurrent;

namespace AlmightyShogun.AspNet.RequestValidation;

/// <summary>
/// Holds the rules for each request type for the life of the process, so reflection and rule construction happen once rather than per
/// request. Both the attribute-only and the fluent paths are cached separately, since a request may use either.
/// </summary>
///
/// <author>Almighty-Shogun</author>
/// <since>Unreleased</since>
internal sealed class ValidationRuleCache
{
    private readonly ConcurrentDictionary<Type, object> _requestRules = new();

    private readonly ConcurrentDictionary<Type, object> _attributeRules = new();

    private readonly ConcurrentDictionary<Type, bool> _hasAttributeRules = new();

    /// <summary>
    /// Reports whether a type declares any attribute rules, so a request with none skips rule building rather than building an empty set.
    /// </summary>
    ///
    /// <param name="requestType">The request type.</param>
    ///
    /// <returns><c>true</c> when the request type has attribute rules; otherwise, <c>false</c>.</returns>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    public bool HasAttributeRules(Type requestType)
        => requestType.IsClass && _hasAttributeRules.GetOrAdd(requestType, AttributeRuleFactory.HasRules);

    /// <summary>
    /// Gets the rules a type's attributes declare, building and caching them on first use.
    /// </summary>
    ///
    /// <typeparam name="TRequest">The request type.</typeparam>
    ///
    /// <returns>The cached attribute validation rules.</returns>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    public IReadOnlyList<IRequestValidationRule<TRequest>> GetAttributeRules<TRequest>() where TRequest : class
        => (IReadOnlyList<IRequestValidationRule<TRequest>>)_attributeRules
            .GetOrAdd(typeof(TRequest), _ => MergeAndDeduplicate(AttributeRuleFactory.CreateRules<TRequest>()));

    /// <summary>
    /// Gets the cached rules for a request, combining what its attributes declare with what its fluent configuration adds.
    /// </summary>
    ///
    /// <typeparam name="TRequest">The request type whose rules are built and then cached against it.</typeparam>
    /// <param name="createFluentRules">
    /// Builds the request-level fluent rules. Invoked only on a cache miss, so a request validated repeatedly pays for
    /// its fluent configuration once rather than per request.
    /// </param>
    ///
    /// <returns>
    /// The merged rules, with rules for the same field combined and duplicate identities removed, so a field declaring the same rule twice
    /// is checked once.
    /// </returns>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    public IReadOnlyList<IRequestValidationRule<TRequest>> GetRequestRules<TRequest>(
        Func<IReadOnlyList<IRequestValidationRule<TRequest>>> createFluentRules
    ) where TRequest : class => (IReadOnlyList<IRequestValidationRule<TRequest>>)_requestRules
        .GetOrAdd(typeof(TRequest), _ => MergeAndDeduplicate(AttributeRuleFactory.CreateRules<TRequest>().Concat(createFluentRules())));

    /// <summary>
    /// Merges rules for the same field and removes duplicate rule identities.
    /// </summary>
    ///
    /// <typeparam name="TRequest">The request type the rules belong to.</typeparam>
    /// <param name="rules">
    /// The rules in declaration order, attributes before fluent, which is the order a field's merged rule keeps.
    /// </param>
    ///
    /// <returns>
    /// One rule per field, each already deduplicated internally, so the same constraint declared by both an attribute and a fluent call
    /// produces a single failure rather than two identical ones.
    /// </returns>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    private static IRequestValidationRule<TRequest>[] MergeAndDeduplicate<TRequest>(
        IEnumerable<IRequestValidationRule<TRequest>> rules
    ) where TRequest : class
    {
        List<IRequestValidationRule<TRequest>> mergedRules = [];

        foreach (IRequestValidationRule<TRequest> rule in rules)
        {
            if (TryMerge(mergedRules, rule)) continue;

            mergedRules.Add(rule);
        }

        foreach (IRequestValidationRule<TRequest> rule in mergedRules)
            rule.DeduplicateRules();

        return [.. mergedRules];
    }

    /// <summary>
    /// Attempts to merge a rule into an existing rule list.
    /// </summary>
    ///
    /// <typeparam name="TRequest">The request type the rules belong to.</typeparam>
    /// <param name="rules">The rules accepted so far, searched for one covering the same field.</param>
    /// <param name="rule">The rule to fold in.</param>
    ///
    /// <returns>
    /// <c>true</c> when an existing rule absorbed it; otherwise <c>false</c> , which is the caller's signal to keep the rule as a new entry
    /// rather than discard it.
    /// </returns>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    private static bool TryMerge<TRequest>(
        List<IRequestValidationRule<TRequest>> rules,
        IRequestValidationRule<TRequest> rule
    ) where TRequest : class => rules.Any(existingRule => existingRule.TryMerge(rule));
}
