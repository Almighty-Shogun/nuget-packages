namespace AlmightyShogun.AspNet.RequestValidation;

/// <summary>
/// One field's rules, seen as a unit the request validator can run. Sits above the property rules so that merging, deduplication, and
/// ordering happen per field rather than per rule.
/// </summary>
///
/// <author>Almighty-Shogun</author>
/// <since>Unreleased</since>
internal interface IRequestValidationRule<TRequest> where TRequest : class
{
    /// <summary>
    /// The public field name failures are reported under, which is the camel-cased form a client sees rather than the property name.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    string FieldName { get; }

    /// <summary>
    /// Folds another rule for the same field into this one, so a field declared by both an attribute and a fluent call yields one rule
    /// rather than two that each report separately.
    /// </summary>
    ///
    /// <param name="rule">The request validation rule to merge.</param>
    ///
    /// <returns><c>true</c> when the rule was merged; otherwise, <c>false</c>.</returns>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    bool TryMerge(IRequestValidationRule<TRequest> rule);

    /// <summary>
    /// Drops rules that are identical to one already held, so the same constraint declared twice fails once.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    void DeduplicateRules();

    /// <summary>
    /// Runs this field's rules in order and records any failures, stopping the field at its first failure rather than reporting every one.
    /// </summary>
    ///
    /// <param name="request">The request being validated, so a rule can read another field as well as its own.</param>
    /// <param name="errors">The bag failures are recorded into, mutated in place rather than returned.</param>
    /// <param name="serviceProvider">The service provider used to resolve validation dependencies.</param>
    /// <param name="cancellationToken">The cancellation token for the validation operation.</param>
    ///
    /// <returns>A value task representing the asynchronous validation operation.</returns>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    ValueTask ValidateAsync(
        TRequest request,
        ValidationBag errors,
        IServiceProvider serviceProvider,
        CancellationToken cancellationToken = default
    );
}
