using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using AlmightyShogun.AspNet.Core;
using AlmightyShogun.AspNet.Localization;
using Microsoft.Extensions.DependencyInjection;

namespace AlmightyShogun.AspNet.RequestValidation;

/// <summary>
/// Builds the standard error body for a binding failure, so a model-state problem and a rule failure return the same shape.
/// </summary>
///
/// <author>Almighty-Shogun</author>
/// <since>Unreleased</since>
internal static class ModelStateValidationResponseFactory
{
    private const int StatusCode = StatusCodes.Status422UnprocessableEntity;

    /// <summary>
    /// Converts model-state entries into field errors and wraps them in the standard body.
    /// </summary>
    ///
    /// <param name="context">The action context, read for the model state entries a binding failure left behind.</param>
    ///
    /// <returns>The validation response result.</returns>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    public static IActionResult Create(ActionContext context)
    {
        var messageResolver = context.HttpContext.RequestServices.GetRequiredService<IMessageResolver>();

        if (ModelStateValidationExtractor.HasBodyLevelError(context))
        {
            return HttpErrorResult.Create(new ValidationErrorResponse
            {
                Code = StatusCode,
                Error = ValidationResponseWriter.ErrorCode,
                ErrorDescription = messageResolver.Resolve("validation.invalid-body", []),
                Errors = new Dictionary<string, ValidationRuleError>()
            });
        }

        var validationResponseFactory = context.HttpContext.RequestServices.GetRequiredService<IValidationResponseFactory>();

        ValidationBag errors = ModelStateValidationExtractor.Extract(context.ModelState);

        return validationResponseFactory.Create(new ValidationResponseContext(context.HttpContext, StatusCode, errors));
    }
}
