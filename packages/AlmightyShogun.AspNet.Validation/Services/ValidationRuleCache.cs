using System.Collections.Concurrent;

namespace AlmightyShogun.AspNet.Validation;

/// <summary>
/// Caches validation rules built from request types and attributes.
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
    /// Checks whether a request type has attribute validation rules.
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
    /// Gets cached attribute validation rules for a request type.
    /// </summary>
    ///
    /// <typeparam name="TRequest">The request type.</typeparam>
    ///
    /// <returns>The cached attribute validation rules.</returns>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    public IReadOnlyList<IRequestValidationRule<TRequest>> GetAttributeRules<TRequest>() where TRequest : class
        => (IReadOnlyList<IRequestValidationRule<TRequest>>)_attributeRules.GetOrAdd(
            typeof(TRequest), _ => MergeAndDeduplicate(AttributeRuleFactory.CreateRules<TRequest>()));

    /// <summary>
    /// Gets cached validation rules by combining attribute rules and request-level fluent rules.
    /// </summary>
    ///
    /// <param name="createFluentRules">The callback that creates request-level fluent rules.</param>
    ///
    /// <typeparam name="TRequest">The request type.</typeparam>
    ///
    /// <returns>The cached combined validation rules.</returns>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    public IReadOnlyList<IRequestValidationRule<TRequest>> GetRequestRules<TRequest>(Func<IReadOnlyList<IRequestValidationRule<TRequest>>> createFluentRules) where TRequest : class
        => (IReadOnlyList<IRequestValidationRule<TRequest>>)_requestRules.GetOrAdd(
            typeof(TRequest), _ => MergeAndDeduplicate(AttributeRuleFactory.CreateRules<TRequest>().Concat(createFluentRules())));

    /// <summary>
    /// Merges rules for the same field and removes duplicate rule identities.
    /// </summary>
    ///
    /// <param name="rules">The rules to merge.</param>
    ///
    /// <typeparam name="TRequest">The request type.</typeparam>
    ///
    /// <returns>The merged and deduplicated rules.</returns>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    private static IRequestValidationRule<TRequest>[] MergeAndDeduplicate<TRequest>(IEnumerable<IRequestValidationRule<TRequest>> rules) where TRequest : class
    {
        List<IRequestValidationRule<TRequest>> mergedRules = [];

        foreach (IRequestValidationRule<TRequest> rule in rules)
        {
            if (TryMerge(mergedRules, rule))
                continue;

            mergedRules.Add(rule);
        }

        foreach (IRequestValidationRule<TRequest> rule in mergedRules)
            rule.DeduplicateRules();

        return mergedRules.ToArray();
    }

    /// <summary>
    /// Attempts to merge a rule into an existing rule list.
    /// </summary>
    ///
    /// <param name="rules">The existing rules.</param>
    /// <param name="rule">The rule to merge.</param>
    ///
    /// <typeparam name="TRequest">The request type.</typeparam>
    ///
    /// <returns><c>true</c> when the rule was merged; otherwise, <c>false</c>.</returns>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    private static bool TryMerge<TRequest>(List<IRequestValidationRule<TRequest>> rules, IRequestValidationRule<TRequest> rule) where TRequest : class
        => rules.Any(existingRule => existingRule.TryMerge(rule));
}
