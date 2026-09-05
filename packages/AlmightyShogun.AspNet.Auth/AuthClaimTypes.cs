namespace AlmightyShogun.AspNet.Auth;

/// <summary>
/// The claim types this package reads off a principal. Nothing here writes them: they are stamped by
/// <c>AlmightyShogun.AspNet.Auth.Credentials</c>, so both packages spell the claim type through these constants rather
/// than through a literal string.
/// </summary>
///
/// <author>Almighty-Shogun</author>
/// <since>4.0.0</since>
public static class AuthClaimTypes
{
    /// <summary>
    /// The claim carrying the caller's public identifier, which this package reads as a <c>Guid</c>.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>4.0.0</since>
    public const string UserId = "userId";

    /// <summary>
    /// The claim carrying one granted permission.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>4.0.0</since>
    public const string Permission = "permission";
}
