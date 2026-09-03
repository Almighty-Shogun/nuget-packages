namespace AlmightyShogun.AspNet.Auth;

/// <summary>
/// The policy naming this package generates and recognises. Nothing else should build a policy name by hand: the
/// provider matches on this prefix, and a name that misses it falls through to the framework's own provider.
/// </summary>
///
/// <author>Almighty-Shogun</author>
/// <since>Unreleased</since>
public static class AuthPolicies
{
    /// <summary>
    /// The prefix identifying a permission-backed policy. A policy named <c>permission:users.read</c> requires the
    /// <c>users.read</c> permission claim.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    public const string PermissionPrefix = "permission:";
}
