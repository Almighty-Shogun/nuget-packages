using Microsoft.AspNetCore.Mvc;

namespace AlmightyShogun.AspNet.Core;

/// <summary>
/// The standardized error body as an MVC result, for a controller or a filter that wants to return an error directly
/// rather than throw a mapped exception and let the handler chain produce it.
/// </summary>
///
/// <remarks>
/// Serialized by the application's configured MVC formatters rather than by <see cref="IHttpErrorResponseWriter"/>, so
/// its content type follows MVC's negotiation and its property casing comes from <c>AddJsonOptions</c>. An error
/// written below MVC goes through the writer instead, which reads the casing configured by
/// <c>ConfigureHttpJsonOptions</c>; configure both when an application moves either away from the default.
/// </remarks>
///
/// <author>Almighty-Shogun</author>
/// <since>Unreleased</since>
public sealed class HttpErrorResult : ObjectResult
{
    /// <summary>
    /// Wraps an error body in a result whose status comes from the body itself.
    /// </summary>
    ///
    /// <param name="response">
    /// The body to return. Its <see cref="HttpErrorResponse.Code"/> becomes
    /// <see cref="ObjectResult.StatusCode"/>, so the status a client reads in the headers and the one it reads in the
    /// body cannot drift apart the way a hand-built <see cref="ObjectResult"/> can.
    /// </param>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    public HttpErrorResult(HttpErrorResponse response) : base(response) => StatusCode = response.Code;
}
