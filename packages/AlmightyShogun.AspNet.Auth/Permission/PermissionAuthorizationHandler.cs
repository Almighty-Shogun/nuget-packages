using Microsoft.AspNetCore.Authorization;

namespace AlmightyShogun.AspNet.Auth;

/// <summary>
/// Authorizes a request when the current principal contains the required permission claim.
/// </summary>
///
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
    /// Reports whether one granted permission satisfies the requirement, which is where a wildcard grant is honored rather
    /// than treated as a literal permission name.
    /// </summary>
    ///
    /// <param name="granted">The permission value held by the principal, which may end in a <c>*</c> wildcard.</param>
    /// <param name="required">The permission the endpoint demands, as written on the attribute.</param>
    ///
    /// <returns>
    /// <c>true</c> when the granted value covers the requirement, whether by matching it or by being broad enough to
    /// include it; otherwise <c>false</c>.
    /// </returns>
    ///
    /// <remarks>
    /// The wildcard is only honored on the granted side. A requirement written as <c>users.*</c> is matched literally,
    /// so an endpoint can never end up accepting less than it asked for.
    /// </remarks>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
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
