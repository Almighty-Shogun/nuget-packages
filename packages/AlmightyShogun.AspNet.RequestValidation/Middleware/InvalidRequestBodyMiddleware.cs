
using Microsoft.AspNetCore.Http;

namespace AlmightyShogun.AspNet.RequestValidation;

/// <summary>
/// Converts supported request-binding exceptions into standardized validation error responses.
/// </summary>
/// <remarks>
/// Handles HTTP 400, 413, and 415 exceptions without modifying responses returned normally by endpoints.
/// </remarks>
/// <author>Almighty-Shogun</author>
/// <since>4.0.0</since>
internal sealed class InvalidRequestBodyMiddleware(RequestDelegate next, ValidationResponseWriter responseWriter)
{
    private const int _statusCode = ValidationErrorResponseFactory.StatusCode;

    /// <summary>
    /// Executes the request pipeline and handles supported bad-request exceptions.
    /// </summary>
    /// <param name="context">The current HTTP context.</param>
    /// <returns>A task representing the middleware operation.</returns>
    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await next(context);
        }
        catch (BadHttpRequestException exception) when (IsUnreadableBody(context, exception))
        {
            await WriteInvalidBodyResponseAsync(context, exception.StatusCode);
        }
    }

    /// <summary>
    /// Checks whether the exception has a supported status and the response has not started.
    /// </summary>
    private static bool IsUnreadableBody(HttpContext context, BadHttpRequestException exception)
    {
        if (context.Response.HasStarted)
            return false;

        return exception.StatusCode is StatusCodes.Status400BadRequest or StatusCodes.Status413PayloadTooLarge
            or StatusCodes.Status415UnsupportedMediaType;
    }

    /// <summary>
    /// Writes the standard error response while preserving the exception's HTTP status.
    /// </summary>
    private Task WriteInvalidBodyResponseAsync(HttpContext context, int statusCode = _statusCode) => responseWriter.WriteAsync(
        context,
        statusCode,
        statusCode == _statusCode ? "validation.invalid-body" : $"http-error.{statusCode}",
        context.RequestAborted);
}
