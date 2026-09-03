using Microsoft.AspNetCore.Authorization;

namespace AlmightyShogun.AspNet.Auth;

/// <summary>
/// Requires the authenticated principal to hold a permission claim satisfying the named permission, on a controller, an
/// action, or a minimal API endpoint.
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
    /// <param name="permission">
    /// The permission the principal must hold. Becomes a policy named <c>permission:{permission}</c>, which the package's
    /// policy provider resolves and caches.
    /// </param>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>2.3.0</since>
    public AuthPermissionAttribute(string permission) => Policy = $"{AuthPolicies.PermissionPrefix}{permission}";
}
