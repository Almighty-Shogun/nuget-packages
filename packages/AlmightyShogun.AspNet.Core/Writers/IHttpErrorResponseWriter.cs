using Microsoft.AspNetCore.Http;

namespace AlmightyShogun.AspNet.Core;

/// <summary>
/// Writes the standardized error response body, so a consumer can return the same body from an endpoint or middleware
/// without an exception being involved. It is not the only path to that body: <see cref="HttpErrorResult"/> carries the
/// same shape through MVC's formatters and never reaches this writer.
/// </summary>
///
/// <author>Almighty-Shogun</author>
/// <since>Unreleased</since>
public interface IHttpErrorResponseWriter
{
    /// <summary>
    /// Writes the error body and sets the status code, so every failure in the application reaches the client as the
    /// same shape no matter which layer produced it.
    /// </summary>
    ///
    /// <param name="context">The context whose response is written to. Left untouched once the response has started.</param>
    /// <param name="statusCode">The status code set on the response and repeated inside the body.</param>
    /// <param name="errorCode">The stable machine-readable error identifier, such as <c>invalid_credentials</c>.</param>
    /// <param name="description">
    /// The human-readable description, already resolved and formatted by the caller. Pass <c>null</c> rather than a
    /// placeholder when there is nothing to say: <see cref="HttpErrorResponse.ErrorDescription"/> then carries
    /// <c>null</c> rather than an empty string, and whether the client sees the field at all follows the JSON options
    /// the host configured.
    /// </param>
    /// <param name="cancellationToken">
    /// Cancels the writing. Pass <c>HttpContext.RequestAborted</c> so a client that disconnects mid-body stops the write
    /// instead of serializing into a dead connection.
    /// </param>
    ///
    /// <returns>
    /// A task that completes once the body has been written, or immediately when the response had already started and
    /// nothing could be written.
    /// </returns>
    ///
    /// <remarks>
    /// Returning quietly rather than throwing on a started response is deliberate: this is called from exception
    /// handlers, where throwing would replace the original failure with one about the failure.
    /// </remarks>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    Task WriteAsync(
        HttpContext context,
        int statusCode,
        string errorCode,
        string? description,
        CancellationToken cancellationToken = default
    );
}
