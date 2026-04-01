using Microsoft.AspNetCore.Authorization;

namespace AlmightyShogun.AspNet.JwtAuth.Permission;

public class PermissionRequirement(string permission) : IAuthorizationRequirement
{
    public string Permission { get; } = permission;
}
