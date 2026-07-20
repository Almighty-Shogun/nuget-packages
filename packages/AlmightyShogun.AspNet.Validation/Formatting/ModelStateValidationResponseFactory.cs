using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using AlmightyShogun.AspNet.Utils;
using Microsoft.Extensions.DependencyInjection;

namespace AlmightyShogun.AspNet.Validation;

/// <summary>
/// Creates standardized validation responses from MVC model-state failures.
/// </summary>
///
/// <author>Almighty-Shogun</author>
/// <since>Unreleased</since>
internal static class ModelStateValidationResponseFactory
{
    private const int _statusCode = StatusCodes.Status422UnprocessableEntity;

    /// <summary>
    /// Creates a validation response from MVC model state.
    /// </summary>
    ///
    /// <param name="context">The MVC action context containing model state.</param>
    ///
    /// <returns>The validation response result.</returns>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    public static IActionResult Create(ActionContext context)
    {
        IMessageResolver messageResolver = context.HttpContext.RequestServices.GetRequiredService<IMessageResolver>();

        if (ModelStateValidationExtractor.HasBodyLevelError(context))
        {
            return HttpErrorResult.Create(new HttpErrorResponse
            {
                Code = _statusCode,
                Error = "validation_error",
                ErrorDescription = messageResolver.Resolve("http-error.invalid-body", [])
            });
        }

        IValidationResponseFactory validationResponseFactory = context.HttpContext.RequestServices.GetRequiredService<IValidationResponseFactory>();
        ValidationBag errors = ModelStateValidationExtractor.Extract(context.ModelState);

        return validationResponseFactory.Create(new ValidationResponseContext(context.HttpContext, _statusCode, errors));
    }
}
