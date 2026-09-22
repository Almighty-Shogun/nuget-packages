using Microsoft.AspNetCore.Http;

namespace AlmightyShogun.AspNet.Core;

/// <summary>
/// Writes standardized HTTP error responses as JSON.
/// </summary>
///
/// <author>Almighty-Shogun</author>
/// <since>4.0.0</since>
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
