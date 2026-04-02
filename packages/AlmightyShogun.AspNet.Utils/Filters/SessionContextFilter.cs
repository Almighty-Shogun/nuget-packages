using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Filters;

namespace AlmightyShogun.AspNet.Utils.Filters;

public class SessionContextFilter : IActionFilter
{
    /// <inheritdoc/>
    public void OnActionExecuting(ActionExecutingContext context)
    {
        HttpContext httpContext = context.HttpContext;

        string? ip = httpContext.Request.Headers["X-Forwarded-For"].FirstOrDefault()
                     ?? httpContext.Connection.RemoteIpAddress?.ToString();

        var sessionCtx = new SessionContext(
            IpAddress: ip,
            UserAgent: httpContext.Request.Headers.UserAgent.ToString()
        );

        httpContext.Items[SessionContext.ItemKey] = sessionCtx;
    }

    /// <inheritdoc/>
    public void OnActionExecuted(ActionExecutedContext context) { }
}
