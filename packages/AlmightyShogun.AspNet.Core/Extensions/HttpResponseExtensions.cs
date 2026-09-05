using Microsoft.AspNetCore.Http;

namespace AlmightyShogun.AspNet.Core;

/// <summary>
/// Deletes response cookies. Deletion travels as a <c>Set-Cookie</c> header, so it has to happen before the response
/// starts.
/// </summary>
///
/// <author>Almighty-Shogun</author>
/// <since>2.3.0</since>
public static class HttpResponseExtensions
{
    /// <summary>
    /// Provides the cookie helpers as extensions on the response.
    /// </summary>
    ///
    /// <param name="httpResponse">
    /// The response being built. Its headers are appended to, but the body is neither written nor completed here, so it
    /// remains the caller's to produce.
    /// </param>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>2.3.0</since>
    extension(HttpResponse httpResponse)
    {
        /// <summary>
        /// Deletes the named cookies by emitting an expired <c>Set-Cookie</c> for each.
        /// </summary>
        ///
        /// <param name="cookieNames">
        /// The cookie names to delete. Blank names are ignored, so a name read from configuration can be passed without
        /// a guard.
        /// </param>
        ///
        /// <exception cref="InvalidOperationException">
        /// The response has already started. There is no guard here, and each name appends a <c>Set-Cookie</c> header,
        /// which Kestrel rejects once the headers are sent. Check <c>HttpResponse.HasStarted</c> first where the call
        /// site cannot rule that out.
        /// </exception>
        ///
        /// <remarks>
        /// The expiry is scoped to the root path and the current host. A cookie written with a different path or domain
        /// is a different cookie to the browser and survives this call; delete it through <c>Cookies.Delete</c> with
        /// matching options instead.
        /// </remarks>
        ///
        /// <author>Almighty-Shogun</author>
        /// <since>2.3.0</since>
        public void DeleteCookies(params string[] cookieNames)
        {
            foreach (string cookieName in cookieNames.Where(cookieName => !string.IsNullOrWhiteSpace(cookieName)))
                httpResponse.Cookies.Delete(cookieName);
        }
    }
}
