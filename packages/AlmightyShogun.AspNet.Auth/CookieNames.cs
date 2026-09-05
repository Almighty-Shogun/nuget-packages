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
    /// The name of the cookie carrying the refresh token, written and deleted by <see cref="HttpResponseExtensions"/>.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>2.3.0</since>
    public const string RefreshToken = "refreshToken";
}
