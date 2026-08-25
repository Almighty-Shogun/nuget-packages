namespace AlmightyShogun.AspNet.RequestValidation;

/// <summary>
/// One rule bound to one property. Rules are built once and reused across requests, so an implementation must hold no per-request state.
/// </summary>
///
/// <author>Almighty-Shogun</author>
/// <since>Unreleased</since>
internal interface IPropertyValidationRule<in TRequest, in TProperty> where TRequest : class
{
    /// <summary>
    /// The band this rule runs in. Presence rules claim the earlier band so a missing field is reported as missing rather than as
    /// malformed.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
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
    /// <returns>The validation rule result.</returns>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    ValueTask<ValidationRuleResult> ValidateAsync(
        TRequest request,
        TProperty? value,
        string field,
        IServiceProvider serviceProvider,
        CancellationToken cancellationToken = default
    );
}
