namespace AlmightyShogun.AspNet.Validation;

/// <summary>
/// Defines a request that can provide validation rules.
/// </summary>
///
/// <author>Almighty-Shogun</author>
/// <since>Unreleased</since>
internal interface IValidatableRequest
{
    /// <summary>
    /// Validates the current request.
    /// </summary>
    ///
    /// <param name="serviceProvider">The service provider used to resolve validation dependencies.</param>
    /// <param name="cancellationToken">The cancellation token for the validation operation.</param>
    ///
    /// <returns>The validation errors found during validation.</returns>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    Task<ValidationBag> ValidateAsync(IServiceProvider serviceProvider, CancellationToken cancellationToken = default);
}
