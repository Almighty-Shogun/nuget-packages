using Microsoft.AspNetCore.Mvc;

namespace AlmightyShogun.AspNet.Utils;

/// <summary>
/// Creates MVC object results for standardized HTTP error responses.
/// </summary>
///
/// <author>Almighty-Shogun</author>
/// <since>Unreleased</since>
public static class HttpErrorResult
{
    /// <summary>
    /// Creates an object result for a standardized HTTP error response.
    /// </summary>
    ///
    /// <param name="response">The HTTP error response.</param>
    ///
    /// <returns>The object result with the response status code.</returns>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    public static ObjectResult Create(HttpErrorResponse response) => new(response)
    {
        StatusCode = response.Code
    };
}
