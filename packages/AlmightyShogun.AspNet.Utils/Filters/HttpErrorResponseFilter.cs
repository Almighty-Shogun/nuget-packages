using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Filters;

namespace AlmightyShogun.AspNet.Utils;

/// <summary>
/// Replaces an MVC error result that carries a status code but nobody with the standardized error response, so a bare
/// <c>NotFound()</c> returns the same shape as an error raised through <see cref="IAppException"/>.
/// </summary>
///
/// <param name="messageResolver">The resolver that turns the <c>http-error.{status}</c> key into a localized description.</param>
///
/// <remarks>
/// A result that already carries a value is left alone, so an action returning its own error body keeps it. This runs as
/// a result filter rather than as middleware because the empty result is only visible before MVC executes it.
/// </remarks>
///
/// <author>Almighty-Shogun</author>
/// <since>Unreleased</since>
internal sealed class HttpErrorResponseFilter(IMessageResolver messageResolver) : IAsyncResultFilter
{
    /// <summary>
    /// Swaps an empty error result for a populated one before the result is executed.
    /// </summary>
    ///
    /// <param name="context">
    /// The context whose <see cref="ResultExecutingContext.Result"/> is replaced in place when it is an empty error.
    /// </param>
    /// <param name="next">
    /// The rest of the result pipeline. Always invoked, including when the result was left untouched, since skipping it
    /// would leave the response unwritten.
    /// </param>
    ///
    /// <returns>A task that completes once the remaining result filters and the result itself have run.</returns>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    public async Task OnResultExecutionAsync(ResultExecutingContext context, ResultExecutionDelegate next)
    {
        if (TryGetEmptyErrorStatusCode(context.Result, out int statusCode))
            context.Result = HttpErrorResult.Create(new HttpErrorResponse
            {
                Code = statusCode,
                Error = HttpErrorCodes.FromStatusCode(statusCode),
                ErrorDescription = messageResolver.Resolve($"http-error.{statusCode}")
            });

        await next();
    }

    /// <summary>
    /// Determines whether a result is an error that would be sent with no body.
    /// </summary>
    ///
    /// <param name="result">
    /// The result the action produced, matched by shape rather than by type name so both of MVC's bare-status results
    /// are recognized.
    /// </param>
    /// <param name="statusCode">
    /// The status code to build the error body from when this returns <c>true</c>; otherwise <c>0</c>.
    /// </param>
    ///
    /// <returns>
    /// <c>true</c> for a <see cref="StatusCodeResult"/> or a null-valued <see cref="ObjectResult"/> carrying an error
    /// status; otherwise <c>false</c>, which includes any result that already has content.
    /// </returns>
    ///
    /// <remarks>
    /// An <see cref="ObjectResult"/> with no status code at all is not treated as an error, because MVC resolves it to
    /// <c>200</c> unless something later overrides it.
    /// </remarks>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    private static bool TryGetEmptyErrorStatusCode(IActionResult result, out int statusCode)
    {
        int resolvedStatusCode = result switch
        {
            StatusCodeResult { StatusCode: var resultStatusCode } when IsErrorStatusCode(resultStatusCode) => resultStatusCode,
            ObjectResult { Value: null, StatusCode: { } objectStatusCode } when IsErrorStatusCode(objectStatusCode) =>
                objectStatusCode,
            _ => 0
        };

        statusCode = resolvedStatusCode;

        return resolvedStatusCode is not 0;
    }

    /// <summary>
    /// Decides whether a status code is one this filter should produce a body for.
    /// </summary>
    ///
    /// <param name="statusCode">The status code the result would be sent with.</param>
    ///
    /// <returns>
    /// <c>true</c> from <c>400</c> upwards, which covers both client and server faults; otherwise <c>false</c>.
    /// </returns>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    private static bool IsErrorStatusCode(int statusCode) => statusCode >= StatusCodes.Status400BadRequest;
}
