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
    public const string PolicyPrefix = "permission:";

    /// <summary>
    /// Stores the default ASP.NET Core policy provider used for non-permission policies.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>2.3.0</since>
    private readonly DefaultAuthorizationPolicyProvider _fallback = new(options);

    /// <inheritdoc />
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>2.3.0</since>
    public Task<AuthorizationPolicy?> GetPolicyAsync(string policyName)
    {
        if (!policyName.StartsWith(PolicyPrefix, StringComparison.OrdinalIgnoreCase))
            return _fallback.GetPolicyAsync(policyName);

        string permission = policyName[PolicyPrefix.Length..];
        AuthorizationPolicy policy = new AuthorizationPolicyBuilder()
            .RequireAuthenticatedUser()
            .AddRequirements(new PermissionRequirement(permission))
            .Build();

        return Task.FromResult<AuthorizationPolicy?>(policy);
    }

    /// <inheritdoc />
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>2.3.0</since>
    public Task<AuthorizationPolicy> GetDefaultPolicyAsync() => _fallback.GetDefaultPolicyAsync();

    /// <inheritdoc />
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>2.3.0</since>
    public Task<AuthorizationPolicy?> GetFallbackPolicyAsync() => _fallback.GetFallbackPolicyAsync();
}
