using Microsoft.AspNetCore.Http;

namespace AlmightyShogun.AspNet.Core;

/// <summary>
/// Writes standardized HTTP error responses.
/// </summary>
///
/// <author>Almighty-Shogun</author>
/// <since>4.0.0</since>
public interface IHttpErrorResponseWriter
{
    /// <summary>
    /// Writes a standardized error response.
    /// </summary>
    ///
    /// <param name="context">The HTTP context.</param>
    /// <param name="statusCode">The HTTP status code.</param>
    /// <param name="errorCode">The machine-readable error code.</param>
    /// <param name="description">The resolved human-readable description, or <c>null</c>.</param>
    /// <param name="cancellationToken">The cancellation token for the write.</param>
    ///
    /// <returns>
    /// A task representing the write operation.
    /// </returns>
    ///
    /// <remarks>
    /// Does nothing if the response has already started.
    /// </remarks>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>4.0.0</since>
    Task WriteAsync(
        HttpContext context,
        int statusCode,
        string errorCode,
        string? description,
        CancellationToken cancellationToken = default
    );
}
