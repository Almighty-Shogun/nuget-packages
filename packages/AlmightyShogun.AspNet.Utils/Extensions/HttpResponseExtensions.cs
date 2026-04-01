using Microsoft.AspNetCore.Http;

namespace AlmightyShogun.AspNet.Utils.Extensions;

public static class HttpResponseExtensions
{
    /// <param name="httpResponse">The <see cref="HttpResponse"/> used to register the functionalities.</param>
    extension(HttpResponse httpResponse)
    {
        /// <summary>
        /// Deletes the specified cookies.
        /// </summary>
        ///
        /// <param name="cookieNames">The name of every cookie that should be deleted.</param>
        ///
        /// <author>Almighty-Shogun</author>
        /// <since>2.3.0</since>
        public void DeleteCookies(params string[] cookieNames)
        {
            foreach (string cookieName in cookieNames)
            {
                if (!string.IsNullOrWhiteSpace(cookieName))
                    httpResponse.Cookies.Delete(cookieName);
            }
        }
    }
}
