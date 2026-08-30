using Microsoft.AspNetCore.Authorization;

namespace AlmightyShogun.AspNet.JwtAuth;

/// <summary>
/// The permission a generated policy demands. Carried as a requirement rather than baked into the policy name, so the
/// handler can compare values instead of parsing the name back apart.
/// </summary>
///
/// <param name="permission">
/// The permission the caller must hold, compared against the principal's permission claims case-insensitively.
/// </param>
///
/// <author>Almighty-Shogun</author>
/// <since>2.3.0</since>
internal sealed class PermissionRequirement(string permission) : IAuthorizationRequirement
{
    /// <summary>
    /// Gets the permission the caller must hold, which the handler compares against each permission claim.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>2.3.0</since>
    public string Permission { get; } = permission;
}
