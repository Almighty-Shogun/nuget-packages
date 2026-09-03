namespace AlmightyShogun.AspNet.Auth;

/// <summary>
/// The claim types this package reads from and writes to a token. Use these constants rather than the literal strings,
/// so a token minted by one package is understood by the other.
/// </summary>
///
/// <author>Almighty-Shogun</author>
/// <since>Unreleased</since>
public static class AuthClaimTypes
{
    /// <summary>
    /// The claim carrying the caller's public identifier. A <c>Guid</c>, not the database key, so a token never leaks how
    /// many accounts exist or where this one falls among them.
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
