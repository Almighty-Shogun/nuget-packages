namespace AlmightyShogun.AspNet.RequestValidation;

/// <summary>
/// A request that declares rules in code, for constraints an attribute cannot express. Its attributes still apply: the two sets are merged
/// per field rather than one replacing the other.
/// </summary>
///
/// <author>Almighty-Shogun</author>
/// <since>Unreleased</since>
internal interface IValidatableRequest
{
    /// <summary>
    /// Runs the rules this request declares together with those its attributes declare, attributes first, merged per field and reordered
    /// so the presence band runs before the value rules.
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
