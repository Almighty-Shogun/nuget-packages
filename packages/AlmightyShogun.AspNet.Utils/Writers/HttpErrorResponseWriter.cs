using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;

namespace AlmightyShogun.AspNet.Utils;

/// <summary>
/// Writes the standardized error response body as JSON, or as RFC 9457 problem details when the application opted in.
/// </summary>
///
/// <param name="errorOptions">The settings deciding which of the two body shapes is written.</param>
///
/// <remarks>
/// Registered as a singleton, so the shape is fixed for the process. <c>ContentLength</c> is cleared before writing,
/// because a status set earlier in the pipeline may have declared a length for a body that never arrived.
/// </remarks>
///
/// <author>Almighty-Shogun</author>
/// <since>Unreleased</since>
internal sealed class HttpErrorResponseWriter(IOptions<HttpErrorSettings> errorOptions) : IHttpErrorResponseWriter
{
    /// <summary>
    /// The body-shape choice, read once at construction. <see cref="IOptions{TOptions}"/> is not held, since switching
    /// error shapes mid-process would change the response contract under clients already parsing it.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    private readonly HttpErrorSettings _settings = errorOptions.Value;

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

        if (_settings.UseProblemDetails)
        {
            await context.Response.WriteAsJsonAsync(
                new HttpProblemDetails
                {
                    Title = errorCode,
                    Status = statusCode,
                    Detail = description,
                    Instance = context.Request.Path.Value
                },
                options: null,
                contentType: "application/problem+json; charset=utf-8",
                cancellationToken
            );

            return;
        }

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
