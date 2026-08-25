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
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    public async ValueTask<object?> InvokeAsync(EndpointFilterInvocationContext context, EndpointFilterDelegate next)
    {
        foreach (object? argument in context.Arguments)
        {
            if (!CanCarryRules(argument)) continue;

            ValidationBag errors = await requestValidator.ValidateAsync(argument, context.HttpContext.RequestAborted);

            if (!errors.HasErrors) continue;

            return Results.Json(
                responseWriter.CreateResponse(errors),
                statusCode: ValidationResponseWriter.StatusCode
            );
        }

        return await next(context);
    }

    /// <summary>
    /// Reports whether an argument is the kind of thing that can carry rules, so the framework values a handler also receives are skipped.
    /// </summary>
    ///
    /// <param name="argument">One bound argument of the endpoint.</param>
    ///
    /// <returns>
    /// <c>false</c> for <c>null</c>, for a value type, for a string, and for the request context and its cancellation token; otherwise
    /// <c>true</c>. Those are never request models, so validating them only costs a reflection pass to learn they declare nothing.
    /// </returns>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    private static bool CanCarryRules(object? argument) => argument is not null
        and not ValueType
        and not string
        and not HttpContext
        and not CancellationToken;
}
