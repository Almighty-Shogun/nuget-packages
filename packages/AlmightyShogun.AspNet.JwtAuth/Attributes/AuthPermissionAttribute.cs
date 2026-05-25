using Microsoft.AspNetCore.Authorization;

namespace AlmightyShogun.AspNet.JwtAuth;

public class AuthPermissionAttribute : AuthorizeAttribute
{
    public AuthPermissionAttribute(string permission)
        => Policy = $"{PermissionPolicyProvider.PolicyPrefix}{permission}";
}
