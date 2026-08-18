using Microsoft.AspNetCore.Mvc;

namespace AlmightyShogun.AspNet.Utils;

/// <summary>
/// Wraps the standardized error body in an MVC result, for a controller that wants to return an error directly rather
/// than throw an <see cref="IAppException"/> and let the handler chain produce it.
/// </summary>
///
/// <author>Almighty-Shogun</author>
/// <since>Unreleased</since>
public static class HttpErrorResult
{
    /// <summary>
    /// Wraps an error body in a result that carries its own status code.
    /// </summary>
    ///
    /// <param name="response">
    /// The body to return. Its <see cref="HttpErrorResponse.Code"/> becomes the response status, so the two cannot
    /// drift apart the way a hand-built <see cref="ObjectResult"/> can.
    /// </param>
    ///
    /// <returns>
    /// An <see cref="ObjectResult"/> holding the body, serialized by the application's configured formatters rather
    /// than by the package writer, so this path ignores <see cref="HttpErrorSettings.UseProblemDetails"/>.
    /// </returns>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    public static ObjectResult Create(HttpErrorResponse response) => new(response) { StatusCode = response.Code };
}
