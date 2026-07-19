using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Filters;

namespace AlmightyShogun.AspNet.Utils;

/// <summary>
/// Converts empty MVC error results into standardized HTTP error response bodies.
/// </summary>
///
/// <author>Almighty-Shogun</author>
/// <since>Unreleased</since>
internal sealed class HttpErrorResponseFilter(IMessageResolver messageResolver) : IAsyncResultFilter
{
    /// <summary>
    /// Converts empty MVC error results into standardized HTTP error responses.
    /// </summary>
    ///
    /// <param name="context">The result execution context.</param>
    /// <param name="next">The next result filter delegate.</param>
    ///
    /// <returns>A task representing the asynchronous filter operation.</returns>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    public async Task OnResultExecutionAsync(ResultExecutingContext context, ResultExecutionDelegate next)
    {
        if (TryGetEmptyErrorStatusCode(context.Result, out int statusCode))
        {
            context.Result = HttpErrorResult.Create(HttpErrorResponseFactory.Create(statusCode, messageResolver));
        }

        await next();
    }

    /// <summary>
    /// Attempts to read an error status code from an empty MVC result.
    /// </summary>
    ///
    /// <param name="result">The MVC result.</param>
    /// <param name="statusCode">The resolved error status code.</param>
    ///
    /// <returns><c>true</c> when the result is an empty error response; otherwise, <c>false</c>.</returns>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    private static bool TryGetEmptyErrorStatusCode(IActionResult result, out int statusCode)
    {
        int resolvedStatusCode = result switch
        {
            StatusCodeResult { StatusCode: var resultStatusCode } when IsErrorStatusCode(resultStatusCode) => resultStatusCode,
            ObjectResult { Value: null, StatusCode: { } objectStatusCode } when IsErrorStatusCode(objectStatusCode) => objectStatusCode,
            _ => 0
        };

        statusCode = resolvedStatusCode;

        return resolvedStatusCode is not 0;
    }

    /// <summary>
    /// Checks whether a status code represents an HTTP error.
    /// </summary>
    ///
    /// <param name="statusCode">The HTTP status code.</param>
    ///
    /// <returns><c>true</c> when the status code is at least <c>400</c>; otherwise, <c>false</c>.</returns>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    private static bool IsErrorStatusCode(int statusCode) => statusCode >= StatusCodes.Status400BadRequest;
}
