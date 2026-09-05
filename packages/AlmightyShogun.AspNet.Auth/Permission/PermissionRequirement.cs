using Microsoft.AspNetCore.Authorization;

namespace AlmightyShogun.AspNet.Auth;

/// <summary>
/// The permission a generated policy demands. Carried as a requirement rather than baked into the policy name, so the
/// handler can compare values instead of parsing the name back apart.
/// </summary>
///
/// <param name="permission">The permission the caller must hold.</param>
///
/// <author>Almighty-Shogun</author>
/// <since>2.3.0</since>
internal sealed class PermissionRequirement(string permission) : IAuthorizationRequirement
{
    /// <summary>
    /// The permission the caller must hold, as written on the attribute that generated the policy.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>2.3.0</since>
    public string Permission { get; } = permission;
}
