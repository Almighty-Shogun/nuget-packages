using Microsoft.AspNetCore.Http;
using AlmightyShogun.AspNet.Utils;

namespace AlmightyShogun.AspNet.Validation;

/// <summary>
/// Provides validation exception middleware behavior.
/// </summary>
///
/// <author>Almighty-Shogun</author>
/// <since>Unreleased</since>
internal sealed class ValidationExceptionMiddleware(RequestDelegate next, IMessageResolver messageResolver)
{
    private const int _statusCode = StatusCodes.Status422UnprocessableEntity;

    /// <summary>
    /// Runs validation exception handling for the current request.
    /// </summary>
    ///
    /// <param name="context">The current HTTP context.</param>
    ///
    /// <returns>A task representing the asynchronous middleware operation.</returns>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await next(context);

            if (ShouldWriteInvalidBodyResponse(context))
                await WriteInvalidBodyResponseAsync(context);
        }
        catch (ValidationException exception) when (!context.Response.HasStarted)
        {
            await WriteValidationExceptionResponseAsync(context, exception);
        }
        catch (BadHttpRequestException) when (!context.Response.HasStarted)
        {
            await WriteInvalidBodyResponseAsync(context);
        }
    }

    /// <summary>
    /// Writes the standardized validation exception response.
    /// </summary>
    ///
    /// <param name="context">The current HTTP context.</param>
    /// <param name="exception">The validation exception to write.</param>
    ///
    /// <returns>A task representing the asynchronous write operation.</returns>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    private Task WriteValidationExceptionResponseAsync(HttpContext context, ValidationException exception)
    {
        context.Response.ContentLength = null;
        context.Response.StatusCode = _statusCode;
        context.Response.ContentType = "application/json; charset=utf-8";

        return context.Response.WriteAsJsonAsync(new ValidationErrorResponse
        {
            Code = _statusCode,
            Error = "validation_error",
            ErrorDescription = messageResolver.Resolve($"http-error.{_statusCode}"),
            Errors = exception.Errors.ToErrorDictionary(messageResolver)
        }, context.RequestAborted);
    }

    /// <summary>
    /// Writes the standardized invalid-body validation response.
    /// </summary>
    ///
    /// <param name="context">The current HTTP context.</param>
    ///
    /// <returns>A task representing the asynchronous write operation.</returns>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    private Task WriteInvalidBodyResponseAsync(HttpContext context)
    {
        context.Response.ContentLength = null;
        context.Response.StatusCode = _statusCode;
        context.Response.ContentType = "application/json; charset=utf-8";

        return context.Response.WriteAsJsonAsync(new HttpErrorResponse
        {
            Code = _statusCode,
            Error = "validation_error",
            ErrorDescription = messageResolver.Resolve("http-error.invalid-body", [])
        }, context.RequestAborted);
    }

    /// <summary>
    /// Determines whether the current response should be replaced with an invalid-body response.
    /// </summary>
    ///
    /// <param name="context">The current HTTP context.</param>
    ///
    /// <returns><c>true</c> when the response is an empty unsupported-media-type body request response; otherwise, <c>false</c>.</returns>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    private static bool ShouldWriteInvalidBodyResponse(HttpContext context)
    {
        if (context.Response.HasStarted || !IsBodyRequest(context.Request))
            return false;

        if (context.Response.ContentLength > 0)
            return false;

        return context.Response.StatusCode == StatusCodes.Status415UnsupportedMediaType;
    }

    /// <summary>
    /// Checks whether a request method normally carries a body.
    /// </summary>
    ///
    /// <param name="request">The HTTP request.</param>
    ///
    /// <returns><c>true</c> for POST, PUT, or PATCH requests; otherwise, <c>false</c>.</returns>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    private static bool IsBodyRequest(HttpRequest request)
    {
        return HttpMethods.IsPost(request.Method)
            || HttpMethods.IsPut(request.Method)
            || HttpMethods.IsPatch(request.Method);
    }
}
