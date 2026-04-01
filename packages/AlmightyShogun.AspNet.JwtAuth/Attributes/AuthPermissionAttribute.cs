using Microsoft.AspNetCore.Authorization;
using AlmightyShogun.AspNet.JwtAuth.Permission;

namespace AlmightyShogun.AspNet.JwtAuth.Attributes;

public class AuthPermissionAttribute : AuthorizeAttribute
{
    public AuthPermissionAttribute(string permission)
        => Policy = $"{PermissionPolicyProvider.PolicyPrefix}{permission}";
}
