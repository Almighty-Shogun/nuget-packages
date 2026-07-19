using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Filters;

namespace AlmightyShogun.AspNet.Utils;

/// <summary>
/// Captures request metadata and stores it in <see cref="HttpContext.Items"/> before controller actions execute.
/// </summary>
///
/// <author>Almighty-Shogun</author>
/// <since>2.2.1</since>
internal sealed class SessionContextFilter : IActionFilter
{
    /// <inheritdoc />
    public void OnActionExecuting(ActionExecutingContext context)
    {
        HttpContext httpContext = context.HttpContext;

        string? ip = httpContext.Request.Headers["X-Forwarded-For"].FirstOrDefault();

        ip ??= httpContext.Connection.RemoteIpAddress?.ToString();

        var sessionCtx = new SessionContext(ip, httpContext.Request.Headers.UserAgent.ToString());

        httpContext.Items[SessionContext.ItemKey] = sessionCtx;
    }

    /// <inheritdoc />
    public void OnActionExecuted(ActionExecutedContext context) { }
}
