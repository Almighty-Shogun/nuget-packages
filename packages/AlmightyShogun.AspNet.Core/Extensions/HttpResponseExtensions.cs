using Microsoft.AspNetCore.Http;

namespace AlmightyShogun.AspNet.Core;

/// <summary>
/// Provides HTTP response cookie extensions.
/// </summary>
///
/// <author>Almighty-Shogun</author>
/// <since>2.3.0</since>
public static class HttpResponseExtensions
{
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
        /// Deletes the specified cookies.
        /// </summary>
        ///
        /// <param name="cookieNames">The cookie names to delete. Blank names are ignored.</param>
        ///
        /// <remarks>
        /// Cookies created with a different path or domain require matching deletion options and are not deleted by
        /// this method.
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
