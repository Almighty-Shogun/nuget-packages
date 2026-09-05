namespace AlmightyShogun.AspNet.RequestValidation;

/// <summary>
/// One rule bound to one property. Rules are built once and reused across requests, so an implementation must hold no per-request state.
/// </summary>
///
/// <typeparam name="TRequest">The request type the rule reads from when it needs a field other than its own.</typeparam>
/// <typeparam name="TProperty">The validated property's type, which is what the rule receives rather than the request.</typeparam>
///
/// <author>Almighty-Shogun</author>
/// <since>4.0.0</since>
internal interface IPropertyValidationRule<in TRequest, in TProperty> where TRequest : class
{
    /// <summary>
    /// The band this rule runs in. Presence rules claim the earlier band so a missing field is reported as missing rather than as
    /// malformed.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>4.0.0</since>
    ValidationRulePriority Priority => ValidationRulePriority.Normal;

    /// <summary>
    /// Checks one property value. Called only after the rules ahead of it in the same band have run, and receives the container so a rule
    /// that needs a service can resolve one.
    /// </summary>
    ///
    /// <param name="request">The request being validated, so a rule can read another field as well as its own.</param>
    /// <param name="value">The value read from the property, already fetched so every rule for the field sees the same one.</param>
    /// <param name="field">The field name being validated.</param>
    /// <param name="serviceProvider">The service provider used to resolve validation dependencies.</param>
    /// <param name="cancellationToken">The cancellation token for the validation operation.</param>
    ///
    /// <returns>
    /// The verdict for this rule. A failure stops the rules that would have followed it, and its <see cref="ValidationRuleResult.Key"/>
    /// and <see cref="ValidationRuleResult.Parameters"/> are what the field reports, except inside a grouped alternative, which discards
    /// them in favour of a single failure of its own.
    /// </returns>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>4.0.0</since>
    ValueTask<ValidationRuleResult> ValidateAsync(
        TRequest request,
        TProperty? value,
        string field,
        IServiceProvider serviceProvider,
        CancellationToken cancellationToken = default
    );
}
