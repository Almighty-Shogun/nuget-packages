using Microsoft.Extensions.Options;
using Microsoft.AspNetCore.Authorization;

namespace AlmightyShogun.AspNet.JwtAuth;

/// <summary>
/// Creates authorization policies for package permission names and delegates all other policies to ASP.NET Core.
/// </summary>
///
/// <param name="options">The authorization options used by the fallback policy provider.</param>
///
/// <author>Almighty-Shogun</author>
/// <since>2.3.0</since>
internal sealed class PermissionPolicyProvider(IOptions<AuthorizationOptions> options) : IAuthorizationPolicyProvider
{
    /// <summary>
    /// Gets the prefix used to identify permission-backed authorization policies.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>2.3.0</since>
    internal const string PolicyPrefix = "permission:";

    /// <summary>
    /// Stores the default ASP.NET Core policy provider used for non-permission policies.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>2.3.0</since>
    private readonly DefaultAuthorizationPolicyProvider _fallback = new(options);

    /// <inheritdoc />
    public async Task<AuthorizationPolicy?> GetPolicyAsync(string policyName)
    {
        if (!policyName.StartsWith(PolicyPrefix, StringComparison.OrdinalIgnoreCase))
            return AddAppAudienceRequirement(await _fallback.GetPolicyAsync(policyName));

        string permission = policyName[PolicyPrefix.Length..];

        AuthorizationPolicy policy = new AuthorizationPolicyBuilder()
            .RequireAuthenticatedUser()
            .AddRequirements(new PermissionRequirement(permission))
            .Build();

        return AddAppAudienceRequirement(policy);
    }

    /// <inheritdoc />
    public async Task<AuthorizationPolicy> GetDefaultPolicyAsync()
        => AddAppAudienceRequirement(await _fallback.GetDefaultPolicyAsync())!;

    /// <inheritdoc />
    public async Task<AuthorizationPolicy?> GetFallbackPolicyAsync()
        => AddAppAudienceRequirement(await _fallback.GetFallbackPolicyAsync());

    /// <summary>
    /// Adds app-audience authorization to a policy when it is not already present.
    /// </summary>
    ///
    /// <param name="policy">The authorization policy to decorate.</param>
    ///
    /// <returns>The decorated authorization policy, or <c>null</c> when no policy was provided.</returns>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    private static AuthorizationPolicy? AddAppAudienceRequirement(AuthorizationPolicy? policy)
    {
        if (policy is null || policy.Requirements.Any(requirement => requirement is AppAudienceRequirement))
            return policy;

        AuthorizationPolicyBuilder builder = new(policy.AuthenticationSchemes.ToArray());

        foreach (IAuthorizationRequirement requirement in policy.Requirements)
            builder.AddRequirements(requirement);

        return builder
            .AddRequirements(new AppAudienceRequirement())
            .Build();
    }
}
