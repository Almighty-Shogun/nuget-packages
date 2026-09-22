using Microsoft.AspNetCore.Mvc;

namespace AlmightyShogun.AspNet.Core;

/// <summary>
/// Represents a standardized HTTP error response as an MVC action result.
/// </summary>
///
/// <author>Almighty-Shogun</author>
/// <since>4.0.0</since>
public sealed class HttpErrorResult : IActionResult
{
    private readonly HttpErrorResponse _response;

    /// <summary>
    /// Creates an HTTP error result from the specified response.
    /// </summary>
    ///
    /// <param name="response">The error response to return.</param>
    ///
    /// <exception cref="ArgumentNullException">
    /// <paramref name="response"/> is <c>null</c>.
    /// </exception>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>4.0.0</since>
    public HttpErrorResult(HttpErrorResponse response) => _response = response ?? throw new ArgumentNullException(nameof(response));


    /// <summary>
    /// Executes the result using the configured MVC formatters.
    /// </summary>
    ///
    /// <param name="context">The action context.</param>
    ///
    /// <returns>A task representing the result execution.</returns>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>4.0.0</since>
    public Task ExecuteResultAsync(ActionContext context)
    {
        ObjectResult result = new(_response)
        {
            StatusCode = _response.Code
        };

        return result.ExecuteResultAsync(context);
    }
}
