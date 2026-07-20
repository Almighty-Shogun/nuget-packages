using Microsoft.AspNetCore.Mvc;
using AlmightyShogun.AspNet.Utils;

namespace AlmightyShogun.AspNet.Validation;

/// <summary>
/// Creates the default standardized validation error response.
/// </summary>
///
/// <author>Almighty-Shogun</author>
/// <since>Unreleased</since>
internal sealed class DefaultValidationResponseFactory(IMessageResolver messageResolver) : IValidationResponseFactory
{
    /// <inheritdoc />
    public IActionResult Create(ValidationResponseContext context) => HttpErrorResult.Create(new ValidationErrorResponse
    {
        Code = context.StatusCode,
        Error = "validation_error",
        ErrorDescription = messageResolver.Resolve($"http-error.{context.StatusCode}"),
        Errors = context.Errors.ToErrorDictionary(messageResolver)
    });
}
