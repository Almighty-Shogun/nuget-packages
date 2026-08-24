using Microsoft.AspNetCore.Http;

namespace AlmightyShogun.AspNet.Utils;

/// <summary>
/// Serializes the standardized error body to JSON and sets the status alongside it, so the two can never disagree
/// about what the failure was.
/// </summary>
///
/// <remarks>
/// Holds no state, so it is registered as a singleton. <c>ContentLength</c> is cleared before writing, because a status
/// set earlier in the pipeline may have declared a length for a body that never arrived.
/// </remarks>
///
/// <author>Almighty-Shogun</author>
/// <since>Unreleased</since>
internal sealed class HttpErrorResponseWriter : IHttpErrorResponseWriter
{
    /// <inheritdoc />
    public async Task WriteAsync(
        HttpContext context,
        int statusCode,
        string errorCode,
        string? description,
        CancellationToken cancellationToken = default
    )
    {
        if (context.Response.HasStarted) return;

        context.Response.ContentLength = null;
        context.Response.StatusCode = statusCode;

        await context.Response.WriteAsJsonAsync(
            new HttpErrorResponse
            {
                Code = statusCode,
                Error = errorCode,
                ErrorDescription = description
            },
            options: null,
            contentType: "application/json; charset=utf-8",
            cancellationToken
        );
    }
}
