using Microsoft.AspNetCore.Authorization;

namespace AlmightyShogun.AspNet.JwtAuth;

/// <summary>
/// Applies a permission-based authorization policy to a controller or action.
/// </summary>
///
/// <author>Almighty-Shogun</author>
/// <since>2.3.0</since>
public sealed class AuthPermissionAttribute : AuthorizeAttribute
{
    /// <summary>
    /// Creates a permission authorization attribute for the provided permission claim value.
    /// </summary>
    ///
    /// <param name="permission">The permission value that must exist in the authenticated user's permission claims.</param>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>2.3.0</since>
    public AuthPermissionAttribute(string permission)
        => Policy = $"{PermissionPolicyProvider.PolicyPrefix}{permission}";
}
