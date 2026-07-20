using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Filters;

namespace AlmightyShogun.AspNet.Validation;

/// <summary>
/// Runs request validation filter behavior in the ASP.NET request pipeline.
/// </summary>
///
/// <author>Almighty-Shogun</author>
/// <since>Unreleased</since>
internal sealed class RequestValidationFilter(RequestValidator requestValidator, IValidationResponseFactory responseFactory)
    : IAsyncActionFilter
{
    /// <summary>
    /// Validates controller action arguments before the action executes.
    /// </summary>
    ///
    /// <param name="context">The action execution context.</param>
    /// <param name="next">The next action filter delegate.</param>
    ///
    /// <returns>A task representing the asynchronous filter operation.</returns>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        foreach (object? argument in context.ActionArguments.Values)
        {
            ValidationBag errors = await requestValidator.ValidateAsync(argument, context.HttpContext.RequestAborted);

            if (!errors.HasErrors)
                continue;

            context.Result = responseFactory.Create(new ValidationResponseContext(context.HttpContext, StatusCodes.Status422UnprocessableEntity, errors));

            return;
        }

        await next();
    }
}
