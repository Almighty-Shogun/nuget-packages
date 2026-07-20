namespace AlmightyShogun.AspNet.Validation;

/// <summary>
/// Defines a validation rule that can validate a full request instance.
/// </summary>
///
/// <author>Almighty-Shogun</author>
/// <since>Unreleased</since>
internal interface IRequestValidationRule<TRequest> where TRequest : class
{
    /// <summary>
    /// Gets the field name validated by this request rule.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    string FieldName { get; }

    /// <summary>
    /// Attempts to merge another request validation rule into this rule.
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
    /// Removes duplicate property validation rules from this request rule.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    void DeduplicateRules();

    /// <summary>
    /// Validates the request and adds failures to the validation bag.
    /// </summary>
    ///
    /// <param name="request">The request instance being validated.</param>
    /// <param name="errors">The validation bag receiving validation failures.</param>
    /// <param name="serviceProvider">The service provider used to resolve validation dependencies.</param>
    /// <param name="cancellationToken">The cancellation token for the validation operation.</param>
    ///
    /// <returns>A value task representing the asynchronous validation operation.</returns>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    ValueTask ValidateAsync(TRequest request, ValidationBag errors, IServiceProvider serviceProvider, CancellationToken cancellationToken = default);
}
