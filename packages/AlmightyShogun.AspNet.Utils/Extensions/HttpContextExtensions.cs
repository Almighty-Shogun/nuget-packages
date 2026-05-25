using Microsoft.AspNetCore.Http;

namespace AlmightyShogun.AspNet.Utils;

public static class HttpContextExtensions
{
    /// <param name="httpContext">The <see cref="HttpContext"/> used to register the functionalities.</param>
    extension(HttpContext httpContext)
    {
        /// <summary>
        /// Retrieves the current request's SessionContext from HttpContext.Items. If the <see cref="SessionContext"/>
        /// does not exists, it will create a new one.
        /// </summary>
        /// 
        /// <returns>The SessionContext stored in <c>HttpContext.Items["SessionContext"]</c>.</returns>
        ///
        /// <author>Almighty-Shogun</author>
        /// <since>2.2.2</since>
        public SessionContext GetSessionContext()
        {
            if (httpContext.Items.TryGetValue(SessionContext.ItemKey, out object? sessionContext) &&
                sessionContext is SessionContext typedSessionContext)
            {
                return typedSessionContext;
            }

            return new SessionContext(
                httpContext.Connection.RemoteIpAddress?.ToString(),
                httpContext.Request.Headers.UserAgent.ToString()
            );
        }
    }
}
