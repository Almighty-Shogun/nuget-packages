using Microsoft.AspNetCore.Http;

namespace AlmightyShogun.AspNet.RequestValidation;

/// <summary>
/// Answers a request whose body could not be read at all with the same shape a failed rule produces, so a caller sees one error contract
/// whether the body was invalid or merely wrong.
/// </summary>
///
/// <param name="next">The rest of the pipeline, run before the response is inspected.</param>
/// <param name="responseWriter">
/// The writer that produces the body, so an unreadable body reaches the client in the same envelope a failed rule does. It resolves the
/// description too: <c>validation.invalid-body</c> when the body itself was unreadable, or the key for whichever status the failure
/// actually carried.
/// </param>
///
/// <remarks>
/// Two paths reach the same response. A malformed body surfaces as <see cref="BadHttpRequestException"/>, which is caught here, while an
/// unsupported content type surfaces as an empty <c>415</c> that the framework produced without throwing, which is why the status is also
/// inspected after the pipeline has run.
/// </remarks>
///
/// <author>Almighty-Shogun</author>
/// <since>Unreleased</since>
internal sealed class InvalidRequestBodyMiddleware(RequestDelegate next, ValidationResponseWriter responseWriter)
{
    /// <summary>
    /// The status an unreadable body defaults to, shared with the per-field validation response so the two cannot drift apart. A failure
    /// that names its own status, such as a body over the size limit, keeps that one instead.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    private const int _statusCode = ValidationErrorResponseFactory.StatusCode;

    /// <summary>
    /// Runs the pipeline and, if an unreadable body escaped it, writes the standard response. Handled after the fact because the failure
    /// can surface as an exception or as a bare status depending on where binding gave up.
    /// </summary>
    ///
    /// <param name="context">The context whose response is written to, left untouched once the response has started.</param>
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
        catch (BadHttpRequestException exception) when (IsUnreadableBody(context, exception))
        {
            await WriteInvalidBodyResponseAsync(context, exception.StatusCode);
        }
    }

    /// <summary>
    /// Reports whether a bad-request failure is one this middleware owns, so an exception raised for some other reason keeps travelling.
    /// </summary>
    ///
    /// <param name="context">The context whose response is inspected for having started.</param>
    /// <param name="exception">The failure to classify, read for the status the framework raised it with.</param>
    ///
    /// <returns><c>true</c> when the failure describes a body the pipeline could not read; otherwise, <c>false</c>.</returns>
    ///
    /// <remarks>
    /// Only the statuses the framework itself raises for an unreadable request are claimed: a malformed request or a binding failure,
    /// and a body past the size limit. Anything else carrying this exception type came from somewhere other than reading the body,
    /// including application code choosing to throw it, and answering that as a validation problem would relabel a failure this
    /// middleware knows nothing about.
    /// </remarks>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    private static bool IsUnreadableBody(HttpContext context, BadHttpRequestException exception)
    {
        if (context.Response.HasStarted)
            return false;

        return exception.StatusCode is StatusCodes.Status400BadRequest or StatusCodes.Status413PayloadTooLarge;
    }

    /// <summary>
    /// Writes the response for a body the pipeline could not read, keeping the status the failure actually carried rather than
    /// reporting everything as a validation problem. A body over the size limit stays a <c>413</c>, so the client is told to send
    /// less instead of to correct its data.
    /// </summary>
    ///
    /// <param name="context">The context whose response is written to, left untouched once the response has started.</param>
    /// <param name="statusCode">
    /// The status to answer with, defaulting to the validation status for the paths that detect an unreadable body themselves.
    /// </param>
    ///
    /// <returns>A task representing the asynchronous write operation.</returns>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    private Task WriteInvalidBodyResponseAsync(HttpContext context, int statusCode = _statusCode) => responseWriter.WriteAsync(
        context,
        statusCode,
        statusCode == _statusCode ? "validation.invalid-body" : $"http-error.{statusCode}",
        context.RequestAborted
    );

    /// <summary>
    /// Determines whether the current response should be replaced with an invalid-body response.
    /// </summary>
    ///
    /// <param name="context">The context whose response is written to, left untouched once the response has started.</param>
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
    /// Reports whether the method is one that carries a body, so a bodyless request is not failed for having none.
    /// </summary>
    ///
    /// <param name="request">The HTTP request.</param>
    ///
    /// <returns><c>true</c> for POST, PUT, or PATCH requests; otherwise, <c>false</c>.</returns>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    private static bool IsBodyRequest(HttpRequest request)
        => HttpMethods.IsPost(request.Method) || HttpMethods.IsPut(request.Method) || HttpMethods.IsPatch(request.Method);
}
