namespace AlmightyShogun.AspNet.Auth;

/// <summary>
/// The policy naming this package generates and recognises. Use these rather than the literal strings, so a rename
/// cannot leave the provider matching on a prefix nothing writes any more.
/// </summary>
///
/// <author>Almighty-Shogun</author>
/// <since>4.0.0</since>
public static class AuthPolicies
{
    /// <summary>
    /// The prefix identifying a permission-backed policy. <see cref="AuthPermissionAttribute"/> builds its policy names by
    /// putting the permission after it.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>4.0.0</since>
    public const string PermissionPrefix = "permission:";
}
