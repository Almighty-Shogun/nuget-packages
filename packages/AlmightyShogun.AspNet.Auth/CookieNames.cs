namespace AlmightyShogun.AspNet.Auth;

/// <summary>
/// The cookie names this package reads and writes. Use these rather than the literal strings, so a rename cannot leave
/// a reader looking for a cookie the writer no longer sets.
/// </summary>
///
/// <author>Almighty-Shogun</author>
/// <since>2.3.0</since>
public static class CookieNames
{
    /// <summary>
    /// The name of the cookie carrying the refresh token. Written <c>HttpOnly</c> and scoped to the root path, so script on
    /// the page cannot read it. Whether it accompanies a given request still depends on the configured <c>SameSite</c> mode
    /// and, for a cookie written over HTTPS, on that request being secure too.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>2.3.0</since>
    public const string RefreshToken = "refreshToken";
}
