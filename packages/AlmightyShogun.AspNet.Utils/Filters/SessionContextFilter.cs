using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Filters;

namespace AlmightyShogun.AspNet.Utils;

/// <summary>
/// Captures the request address and User-Agent into <see cref="HttpContext.Items"/> before an action runs, so repeated
/// calls to <c>GetSessionContext</c> within one request share a single captured value instead of rebuilding it.
/// </summary>
///
/// <remarks>
/// Registered globally by <c>AddSessionContextFilter</c>. Because it is an action filter, a request short-circuited by
/// middleware never reaches it; <c>GetSessionContext</c> falls back to building the context on demand in that case.
/// </remarks>
///
/// <author>Almighty-Shogun</author>
/// <since>2.2.1</since>
internal sealed class SessionContextFilter : IActionFilter
{
    /// <inheritdoc />
    public void OnActionExecuting(ActionExecutingContext context)
    {
        HttpContext httpContext = context.HttpContext;

        httpContext.Items[SessionContext.ItemKey] = httpContext.CreateSessionContext();
    }

    /// <inheritdoc />
    public void OnActionExecuted(ActionExecutedContext context) { }
}
