using Microsoft.AspNetCore.Http;
using AlmightyShogun.AspNet.Utils;

namespace AlmightyShogun.AspNet.Validation;

/// <summary>
/// Runs endpoint validation filter behavior in the ASP.NET request pipeline.
/// </summary>
///
/// <author>Almighty-Shogun</author>
/// <since>Unreleased</since>
internal sealed class EndpointValidationFilter(RequestValidator requestValidator, IMessageResolver messageResolver)
    : IEndpointFilter
{
    /// <summary>
    /// Validates minimal API endpoint arguments before the endpoint executes.
    /// </summary>
    ///
    /// <param name="context">The endpoint filter invocation context.</param>
    /// <param name="next">The next endpoint filter delegate.</param>
    ///
    /// <returns>The endpoint result or validation error response.</returns>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    public async ValueTask<object?> InvokeAsync(EndpointFilterInvocationContext context, EndpointFilterDelegate next)
    {
        foreach (object? argument in context.Arguments)
        {
            ValidationBag errors = await requestValidator.ValidateAsync(argument, context.HttpContext.RequestAborted);

            if (!errors.HasErrors)
                continue;

            const int statusCode = StatusCodes.Status422UnprocessableEntity;

            return Results.Json(new ValidationErrorResponse
            {
                Code = statusCode,
                Error = "validation_error",
                ErrorDescription = messageResolver.Resolve($"http-error.{statusCode}"),
                Errors = errors.ToErrorDictionary(messageResolver)
            }, statusCode: statusCode);
        }

        return await next(context);
    }
}
