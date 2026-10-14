using Microsoft.AspNetCore.Authorization;

namespace AlmightyShogun.AspNet.Auth;

/// <summary>
/// Represents a required authorization permission.
/// </summary>
/// <param name="permission">The required permission.</param>
/// <author>Almighty-Shogun</author>
/// <since>2.3.0</since>
internal sealed class PermissionRequirement(string permission) : IAuthorizationRequirement
{
    /// <summary>
    /// The permission required for authorization.
    /// </summary>
    /// <author>Almighty-Shogun</author>
    /// <since>2.3.0</since>
    public string Permission { get; } = permission;
}
