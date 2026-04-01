using Microsoft.AspNetCore.Http;

namespace AlmightyShogun.AspNet.Utils.Extensions;

public static class HttpContextExtensions
{
    /// <param name="httpContext">The <see cref="HttpContext"/> used to register the functionalities.</param>
    extension(HttpContext httpContext)
    {
        /// <summary>
        /// Retrieves the current request's SessionContext from HttpContext.Items.
        /// </summary>
        /// 
        /// <returns>The SessionContext stored in <c>HttpContext.Items["SessionContext"]</c>.</returns>
        ///
        /// <author>Almighty-Shogun</author>
        /// <since>2.2.2</since>
        public SessionContext GetSessionContext() => (SessionContext)httpContext.Items["SessionContext"]!;
    }
}
