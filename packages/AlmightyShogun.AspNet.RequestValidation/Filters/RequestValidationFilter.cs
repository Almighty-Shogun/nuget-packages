using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Filters;

namespace AlmightyShogun.AspNet.RequestValidation;

/// <summary>
/// Validates a controller action's bound arguments before the action runs. Registered globally, so no controller opts in individually.
/// </summary>
///
/// <author>Almighty-Shogun</author>
/// <since>Unreleased</since>
internal sealed class RequestValidationFilter(
    RequestValidator requestValidator,
    IValidationResponseFactory responseFactory
) : IAsyncActionFilter
{
    /// <summary>
    /// Validates each bound argument, replacing the result with the standard error body rather than invoking the action when one fails.
    /// </summary>
    ///
    /// <param name="context">The action about to run, whose bound arguments are validated and whose result is replaced on failure.</param>
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

            if (!errors.HasErrors) continue;

            context.Result = responseFactory.Create(
                new ValidationResponseContext(context.HttpContext, StatusCodes.Status422UnprocessableEntity, errors)
            );

            return;
        }

        await next();
    }
}
