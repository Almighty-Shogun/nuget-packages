
using Microsoft.Extensions.Options;
using System.Collections.Concurrent;
using Microsoft.AspNetCore.Authorization;

namespace AlmightyShogun.AspNet.Auth;

/// <summary>
/// Resolves permission-based policies and delegates other policies to ASP.NET Core.
/// </summary>
/// <param name="options">The authorization options.</param>
/// <author>Almighty-Shogun</author>
/// <since>2.3.0</since>
internal sealed class PermissionPolicyProvider(IOptions<AuthorizationOptions> options) : IAuthorizationPolicyProvider
{
    /// <summary>
    /// The default provider for non-permission policies.
    /// </summary>
    /// <author>Almighty-Shogun</author>
    /// <since>2.3.0</since>
    private readonly DefaultAuthorizationPolicyProvider _fallback = new(options);

    /// <summary>
    /// Caches resolved authorization policies.
    /// </summary>
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

            if (string.IsNullOrWhiteSpace(permission))
                throw new ArgumentException(
                    "Permission name cannot be empty or whitespace.",
                    nameof(policyName));

            policy = new AuthorizationPolicyBuilder().RequireAuthenticatedUser()
                .AddRequirements(new PermissionRequirement(permission))
                .Build();
        }
        else
        {
            policy = await _fallback.GetPolicyAsync(policyName);
        }

        return policy is null ? null : _policies.GetOrAdd(policyName, policy);
    }

    /// <inheritdoc />
    public Task<AuthorizationPolicy> GetDefaultPolicyAsync() => _fallback.GetDefaultPolicyAsync();

    /// <inheritdoc />
    public Task<AuthorizationPolicy?> GetFallbackPolicyAsync() => _fallback.GetFallbackPolicyAsync();
}
