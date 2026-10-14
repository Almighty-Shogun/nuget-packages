using Microsoft.AspNetCore.Authorization;

namespace AlmightyShogun.AspNet.Auth;

/// <summary>
/// Requires an authenticated user to have the specified permission.
/// </summary>
/// <author>Almighty-Shogun</author>
/// <since>2.3.0</since>
public sealed class AuthPermissionAttribute : AuthorizeAttribute
{
    /// <summary>
    /// Creates an authorization attribute for the specified permission.
    /// </summary>
    /// <param name="permission">The required permission.</param>
    /// <exception cref="ArgumentException">
    /// Thrown when the permission is null, empty, or whitespace.
    /// </exception>
    /// <author>Almighty-Shogun</author>
    /// <since>2.3.0</since>
    public AuthPermissionAttribute(string permission)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(permission);
        Policy = $"{AuthPolicies.PermissionPrefix}{permission}";
    }
}
