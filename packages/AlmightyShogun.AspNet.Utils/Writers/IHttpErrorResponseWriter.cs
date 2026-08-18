using Microsoft.AspNetCore.Http;

namespace AlmightyShogun.AspNet.Utils;

/// <summary>
/// Writes the standardized error response body. This is the single place the error shape is produced, so a consumer can
/// return the same body from an endpoint or middleware without an exception being involved.
/// </summary>
///
/// <author>Almighty-Shogun</author>
/// <since>Unreleased</since>
public interface IHttpErrorResponseWriter
{
    /// <summary>
    /// Writes the error body and sets the status code, choosing the package shape or RFC 9457 problem details according
    /// to configuration, so a caller never has to know which one the application returns.
    /// </summary>
    ///
    /// <param name="context">The context whose response is written to. Left untouched once the response has started.</param>
    /// <param name="statusCode">The status code set on the response and repeated inside the body.</param>
    /// <param name="errorCode">The stable machine-readable error identifier, such as <c>invalid_credentials</c>.</param>
    /// <param name="description">
    /// The human-readable description, already resolved and formatted by the caller. Pass <c>null</c> to omit it rather
    /// than passing a placeholder, since the field is dropped when absent.
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
