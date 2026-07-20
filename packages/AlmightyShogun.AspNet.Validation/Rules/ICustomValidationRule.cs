namespace AlmightyShogun.AspNet.Validation;

/// <summary>
/// Defines a custom validation rule that can be resolved from dependency injection.
/// </summary>
///
/// <author>Almighty-Shogun</author>
/// <since>Unreleased</since>
public interface ICustomValidationRule<in TRequest, in TProperty> where TRequest : class
{
    /// <summary>
    /// Validates a property value using custom rule logic.
    /// </summary>
    ///
    /// <param name="request">The request instance being validated.</param>
    /// <param name="value">The property value being validated.</param>
    /// <param name="cancellationToken">The cancellation token for the validation operation.</param>
    ///
    /// <returns>The validation rule result.</returns>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    Task<ValidationRuleResult> ValidateAsync(TRequest request, TProperty? value, CancellationToken cancellationToken = default);
}
