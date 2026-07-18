using Microsoft.AspNetCore.Authorization;

namespace AlmightyShogun.AspNet.JwtAuth;

/// <summary>
/// Represents the permission value required by a generated authorization policy.
/// </summary>
///
/// <param name="permission">The permission claim value required for authorization.</param>
///
/// <author>Almighty-Shogun</author>
/// <since>2.3.0</since>
internal sealed class PermissionRequirement(string permission) : IAuthorizationRequirement
{
    /// <summary>
    /// Gets the permission claim value required by the policy.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>2.3.0</since>
    public string Permission { get; } = permission;
}
