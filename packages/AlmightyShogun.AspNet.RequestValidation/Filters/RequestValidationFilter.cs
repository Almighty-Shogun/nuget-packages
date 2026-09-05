using Microsoft.AspNetCore.Mvc.Filters;

namespace AlmightyShogun.AspNet.RequestValidation;

/// <summary>
/// Validates a controller action's bound arguments before the action runs. Registered globally, so no controller opts in individually.
/// </summary>
///
/// <param name="requestValidator">The validator each bound argument is offered to.</param>
/// <param name="responseWriter">The writer that shapes the gathered failures into the result sent in the action's place.</param>
///
/// <author>Almighty-Shogun</author>
/// <since>4.0.0</since>
internal sealed class RequestValidationFilter(
    RequestValidator requestValidator,
    ValidationResponseWriter responseWriter
) : IAsyncActionFilter
{
    /// <summary>
    /// Validates each bound argument, replacing the result with the standard error body rather than invoking the action when one fails.
    /// </summary>
    ///
    /// <param name="context">The action about to run, whose bound arguments are validated and whose result is replaced on failure.</param>
    /// <param name="next">The rest of the pipeline, invoked only when every argument passed.</param>
    ///
    /// <returns>A task that completes once the action has run, or once a failing argument has replaced the result instead.</returns>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>4.0.0</since>
    public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        foreach (object? argument in context.ActionArguments.Values)
        {
            ValidationBag errors = await requestValidator.ValidateAsync(argument, context.HttpContext.RequestAborted);

            if (!errors.HasErrors) continue;

            context.Result = responseWriter.CreateResult(errors);

            return;
        }

        await next();
    }
}
