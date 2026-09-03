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
    /// The name of the cookie carrying the refresh token. Written <c>HttpOnly</c>, so script on the page cannot read it
    /// even though it is sent on every request to the origin.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>2.3.0</since>
    public const string RefreshToken = "refreshToken";
}
