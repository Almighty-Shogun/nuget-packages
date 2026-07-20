namespace AlmightyShogun.AspNet.Validation;

/// <summary>
/// Defines a validation rule for a single request property.
/// </summary>
///
/// <author>Almighty-Shogun</author>
/// <since>Unreleased</since>
internal interface IPropertyValidationRule<in TRequest, in TProperty> where TRequest : class
{
    /// <summary>
    /// Gets the priority used to order this validation rule.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    ValidationRulePriority Priority => ValidationRulePriority.Normal;

    /// <summary>
    /// Validates a property value.
    /// </summary>
    ///
    /// <param name="request">The request instance being validated.</param>
    /// <param name="value">The property value being validated.</param>
    /// <param name="field">The field name being validated.</param>
    /// <param name="serviceProvider">The service provider used to resolve validation dependencies.</param>
    /// <param name="cancellationToken">The cancellation token for the validation operation.</param>
    ///
    /// <returns>The validation rule result.</returns>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    ValueTask<ValidationRuleResult> ValidateAsync(TRequest request, TProperty? value, string field, IServiceProvider serviceProvider, CancellationToken cancellationToken = default);
}
