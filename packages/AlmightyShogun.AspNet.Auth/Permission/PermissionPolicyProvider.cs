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
    /// default and fallback policies, which pass through unchanged.
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
    /// <since>4.0.0</since>
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

            policy = new AuthorizationPolicyBuilder().RequireAuthenticatedUser()
                .AddRequirements(new PermissionRequirement(permission))
                .Build();
        }
        else
        {
            policy = await _fallback.GetPolicyAsync(policyName);
        }

        return _policies.GetOrAdd(policyName, policy);
    }

    /// <inheritdoc />
    public Task<AuthorizationPolicy> GetDefaultPolicyAsync() => _fallback.GetDefaultPolicyAsync();

    /// <inheritdoc />
    public Task<AuthorizationPolicy?> GetFallbackPolicyAsync() => _fallback.GetFallbackPolicyAsync();
}
