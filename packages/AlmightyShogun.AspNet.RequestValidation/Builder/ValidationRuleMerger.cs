namespace AlmightyShogun.AspNet.RequestValidation;

/// <summary>
/// Combines the rules declared for one request into the set that runs. Rules for the same field are folded into one, duplicates are
/// dropped, and each field's rules are ordered so presence is settled before value.
/// </summary>
///
/// <remarks>
/// Composition rather than caching: this decides what the rule set for a request type <em>is</em> , and
/// <see cref="ValidationRuleCache"/> decides only how long the answer is kept.
/// </remarks>
///
/// <author>Almighty-Shogun</author>
/// <since>Unreleased</since>
internal static class ValidationRuleMerger
{
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
    public static IRequestValidationRule<TRequest>[] Merge<TRequest>(
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
    /// <c>true</c> when an existing rule absorbed it; otherwise <c>false</c> , which is the caller's signal to keep the rule as a new
    /// entry rather than discard it.
    /// </returns>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    private static bool TryMerge<TRequest>(
        List<IRequestValidationRule<TRequest>> rules,
        IRequestValidationRule<TRequest> rule
    ) where TRequest : class => rules.Any(existingRule => existingRule.TryMerge(rule));
}
