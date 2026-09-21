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
/// <since>4.0.0</since>
public sealed class HttpErrorResult : IActionResult
{
    private readonly HttpErrorResponse _response;
    
    public HttpErrorResult(HttpErrorResponse response)
    {
        _response = response ?? throw new ArgumentNullException(nameof(response));
    }


    public Task ExecuteResultAsync(ActionContext context)
    {
        ObjectResult result = new(_response)
        {
            StatusCode = _response.Code
        };

        return result.ExecuteResultAsync(context);
    }
}
