namespace AlmightyShogun.AspNet.RequestValidation;

/// <summary>
/// A request that declares its own rules rather than relying on attributes, for constraints an attribute cannot express.
/// </summary>
///
/// <author>Almighty-Shogun</author>
/// <since>Unreleased</since>
internal interface IValidatableRequest
{
    /// <summary>
    /// Runs the rules this request declares, in the order the configuration added them.
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
