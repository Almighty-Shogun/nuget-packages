namespace AlmightyShogun.AspNet.RequestValidation;

/// <summary>
/// Defines a custom validation rule that can be resolved from dependency injection.
/// </summary>
///
/// <author>Almighty-Shogun</author>
/// <since>Unreleased</since>
public interface ICustomValidationRule<in TRequest, in TProperty> where TRequest : class
{
    /// <summary>
    /// Validates a property with logic an application supplies, resolved from the container so it can depend on services the built-in rules
    /// cannot.
    /// </summary>
    ///
    /// <param name="request">The request being validated, so a rule can read another field as well as its own.</param>
    /// <param name="value">The value read from the property, already fetched so every rule for the field sees the same one.</param>
    /// <param name="cancellationToken">The cancellation token for the validation operation.</param>
    ///
    /// <returns>The validation rule result.</returns>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    Task<ValidationRuleResult> ValidateAsync(TRequest request, TProperty? value, CancellationToken cancellationToken = default);
}
