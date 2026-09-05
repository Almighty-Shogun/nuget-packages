using Microsoft.Extensions.Options;
using System.Collections.Concurrent;
using Microsoft.AspNetCore.Authorization;

namespace AlmightyShogun.AspNet.Auth;

/// <summary>
/// Creates authorization policies for package permission names and delegates all other policies to ASP.NET Core.
/// </summary>
///
/// <param name="options">
/// The framework's authorization options, handed to the fallback provider so a policy this package does not generate is
/// still resolved the way the application declared it.
/// </param>
///
/// <author>Almighty-Shogun</author>
/// <since>2.3.0</since>
internal sealed class PermissionPolicyProvider(IOptions<AuthorizationOptions> options) : IAuthorizationPolicyProvider
{
    /// <summary>
    /// The framework's own provider, asked for every policy name that does not carry the permission prefix, and for the
    /// default and fallback policies, which this provider only decorates rather than builds.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>2.3.0</since>
    private readonly DefaultAuthorizationPolicyProvider _fallback = new(options);

    /// <summary>
    /// Caches built policies. A policy is a pure function of its name, and a custom provider gets none of the framework
    /// caching the default provider enjoys, so without this a policy is rebuilt for every request to every endpoint.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    private readonly ConcurrentDictionary<string, AuthorizationPolicy?> _policies = new(StringComparer.Ordinal);

    /// <inheritdoc />
    public async Task<AuthorizationPolicy?> GetPolicyAsync(string policyName)
    {
        if (_policies.TryGetValue(policyName, out AuthorizationPolicy? cached))
            return cached;

        AuthorizationPolicy? policy;

        if (policyName.StartsWith(AuthPolicies.PermissionPrefix, StringComparison.OrdinalIgnoreCase))
        {
            string permission = policyName[AuthPolicies.PermissionPrefix.Length..];

            policy = AddAppAudienceRequirement(new AuthorizationPolicyBuilder().RequireAuthenticatedUser()
                .AddRequirements(new PermissionRequirement(permission))
                .Build());
        }
        else
        {
            policy = AddAppAudienceRequirement(await _fallback.GetPolicyAsync(policyName));
        }

        return _policies.GetOrAdd(policyName, policy);
    }

    /// <inheritdoc />
    public async Task<AuthorizationPolicy> GetDefaultPolicyAsync()
        => AddAppAudienceRequirement(await _fallback.GetDefaultPolicyAsync())
            ?? throw new InvalidOperationException("The fallback provider returned no default authorization policy.");

    /// <inheritdoc />
    public async Task<AuthorizationPolicy?> GetFallbackPolicyAsync() => AddAppAudienceRequirement(await _fallback.GetFallbackPolicyAsync());

    /// <summary>
    /// Adds the app-audience requirement to a policy unless it already carries one, so every policy resolved through this
    /// provider enforces scoping without each one having to ask for it. A policy an application builds and hands to the
    /// pipeline directly never passes through here and is not decorated.
    /// </summary>
    ///
    /// <param name="policy">The policy to decorate, or <c>null</c> when the fallback provider produced none.</param>
    ///
    /// <returns>
    /// The policy with the requirement added, the original when it already had one, or <c>null</c> when none was given.
    /// </returns>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    private static AuthorizationPolicy? AddAppAudienceRequirement(AuthorizationPolicy? policy)
    {
        if (policy is null || policy.Requirements.Any(requirement => requirement is AppAudienceRequirement))
            return policy;

        AuthorizationPolicyBuilder builder = new([.. policy.AuthenticationSchemes]);

        foreach (IAuthorizationRequirement requirement in policy.Requirements)
            builder.AddRequirements(requirement);

        return builder.AddRequirements(new AppAudienceRequirement()).Build();
    }
}
