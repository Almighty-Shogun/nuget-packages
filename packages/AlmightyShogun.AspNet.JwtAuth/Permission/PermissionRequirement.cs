using Microsoft.AspNetCore.Authorization;

namespace AlmightyShogun.AspNet.JwtAuth;

internal class PermissionRequirement(string permission) : IAuthorizationRequirement
{
    public string Permission { get; } = permission;
}
