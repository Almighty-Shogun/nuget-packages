using Microsoft.AspNetCore.Mvc;
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
    /// <summary>
    /// Converts model-state entries into field errors and wraps them in the standard body.
    /// </summary>
    ///
    /// <param name="context">The action context, read for the model state entries a binding failure left behind.</param>
    ///
    /// <returns>
    /// The result. A failure against the body as a whole reports as an unreadable body with no field detail, since there is no field it
    /// belongs to; anything else reports per field.
    /// </returns>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    public static IActionResult Create(ActionContext context)
    {
        var messageResolver = context.HttpContext.RequestServices.GetRequiredService<IMessageResolver>();

        if (ModelStateValidationExtractor.HasBodyLevelError(context))
            return new HttpErrorResult(ValidationErrorResponseFactory.Create(messageResolver, "validation.invalid-body"));

        ValidationBag errors = ModelStateValidationExtractor.Extract(context.ModelState);

        return new HttpErrorResult(ValidationErrorResponseFactory.Create(messageResolver, errors));
    }
}
