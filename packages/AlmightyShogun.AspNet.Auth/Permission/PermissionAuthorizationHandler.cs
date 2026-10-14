using Microsoft.AspNetCore.Authorization;

namespace AlmightyShogun.AspNet.Auth;

/// <summary>
/// Authorizes users based on their granted permissions.
/// </summary>
/// <author>Almighty-Shogun</author>
/// <since>2.3.0</since>
internal sealed class PermissionAuthorizationHandler : AuthorizationHandler<PermissionRequirement>
{
    /// <inheritdoc />
    protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, PermissionRequirement requirement)
    {
        bool hasPermission = context.User.FindAll(AuthClaimTypes.Permission)
            .Any(claim => Satisfies(claim.Value, requirement.Permission));

        if (hasPermission)
            context.Succeed(requirement);

        return Task.CompletedTask;
    }

    /// <summary>
    /// Checks whether a granted permission satisfies a requirement, including wildcard grants.
    /// </summary>
    /// <param name="granted">The granted permission.</param>
    /// <param name="required">The required permission.</param>
    /// <returns>Whether the permission requirement is satisfied.</returns>
    /// <remarks>
    /// Wildcards use the <c>.*</c> suffix and apply only to granted permissions.
    /// </remarks>
    /// <author>Almighty-Shogun</author>
    /// <since>4.0.0</since>
    private static bool Satisfies(string granted, string required)
    {
        if (granted.Equals(required, StringComparison.OrdinalIgnoreCase))
            return true;

        if (!granted.EndsWith(".*", StringComparison.Ordinal))
            return false;

        string prefix = granted[..^1];

        return required.StartsWith(prefix, StringComparison.OrdinalIgnoreCase);
    }
}
