using Microsoft.AspNetCore.Authorization;

namespace AlmightyShogun.AspNet.JwtAuth;

/// <summary>
/// Authorizes a request when the current principal contains the required permission claim.
/// </summary>
///
/// <author>Almighty-Shogun</author>
/// <since>2.3.0</since>
internal sealed class PermissionAuthorizationHandler : AuthorizationHandler<PermissionRequirement>
{
    /// <inheritdoc />
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>2.3.0</since>
    protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, PermissionRequirement requirement)
    {
        bool hasPermission = context.User
            .FindAll("permission")
            .Any(c => c.Value == requirement.Permission);

        if (hasPermission)
            context.Succeed(requirement);

        return Task.CompletedTask;
    }
}
