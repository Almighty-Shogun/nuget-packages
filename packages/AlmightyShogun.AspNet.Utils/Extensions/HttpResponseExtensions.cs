using Microsoft.AspNetCore.Http;

namespace AlmightyShogun.AspNet.Utils;

/// <summary>
/// Provides helpers for deleting HTTP response cookies.
/// </summary>
///
/// <author>Almighty-Shogun</author>
/// <since>2.3.0</since>
public static class HttpResponseExtensions
{
    /// <param name="httpResponse">The <see cref="HttpResponse"/> used to register the functionalities.</param>
    extension(HttpResponse httpResponse)
    {
        /// <summary>
        /// Deletes the specified cookies.
        /// </summary>
        ///
        /// <param name="cookieNames">The cookie names to delete. Blank names are ignored.</param>
        ///
        /// <author>Almighty-Shogun</author>
        /// <since>2.3.0</since>
        public void DeleteCookies(params string[] cookieNames)
        {
            foreach (string cookieName in cookieNames.Where(cookieName => !string.IsNullOrWhiteSpace(cookieName)))
            {
                httpResponse.Cookies.Delete(cookieName);
            }
        }
    }
}
