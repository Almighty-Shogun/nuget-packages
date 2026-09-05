namespace AlmightyShogun.AspNet.Auth;

/// <summary>
/// The claim types this package reads off a principal. Nothing here writes them: they are stamped by
/// <c>AlmightyShogun.AspNet.Auth.Credentials</c>, so both packages spell the claim type through these constants rather
/// than through a literal string.
/// </summary>
///
/// <author>Almighty-Shogun</author>
/// <since>Unreleased</since>
public static class AuthClaimTypes
{
    /// <summary>
    /// The claim carrying the caller's public identifier. A <c>Guid</c>, not the database key, so a token never leaks how
    /// many accounts exist.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    public const string UserId = "userId";

    /// <summary>
    /// The claim carrying one granted permission. A principal holds one of these per permission, which is what lets a
    /// policy test for a single value rather than parse a list.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    public const string Permission = "permission";
}
