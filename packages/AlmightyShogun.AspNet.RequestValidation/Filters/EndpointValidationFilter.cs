using Microsoft.AspNetCore.Http;

namespace AlmightyShogun.AspNet.RequestValidation;

/// <summary>
/// Validates a minimal API endpoint's bound arguments before the handler runs, which is the endpoint-filter counterpart of the MVC filter.
/// </summary>
///
/// <author>Almighty-Shogun</author>
/// <since>Unreleased</since>
internal sealed class EndpointValidationFilter(
    RequestValidator requestValidator,
    ValidationResponseWriter responseWriter
) : IEndpointFilter
{
    /// <summary>
    /// Validates each bound argument, short-circuiting with the standard error body rather than invoking the handler when one fails.
    /// </summary>
    ///
    /// <param name="context">The invocation, whose bound arguments are the things validated.</param>
    /// <param name="next">The rest of the pipeline, invoked only when every argument passed.</param>
    ///
    /// <returns>The handler's own result, or the standard validation body when an argument failed.</returns>
    ///
    /// <remarks>
    /// Every argument is offered, the framework values a handler also receives included. Deciding which of them could carry rules is left
    /// to the rule cache, which answers from what a type actually declares and remembers the answer, rather than to a list of types to
    /// skip here. Such a list can only ever name a few of them: a logger, a principal, and every injected service would all have to
    /// appear on it.
    /// </remarks>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    public async ValueTask<object?> InvokeAsync(EndpointFilterInvocationContext context, EndpointFilterDelegate next)
    {
        foreach (object? argument in context.Arguments)
        {
            ValidationBag errors = await requestValidator.ValidateAsync(argument, context.HttpContext.RequestAborted);

            if (!errors.HasErrors) continue;

            return Results.Json(
                responseWriter.CreateResponse(errors),
                statusCode: ValidationErrorResponseFactory.StatusCode
            );
        }

        return await next(context);
    }
}
