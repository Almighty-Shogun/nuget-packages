using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.AspNetCore.Authorization;

namespace AlmightyShogun.AspNet.JwtAuth;

/// <summary>
/// Authorizes requests by ensuring the current token audience matches the resolved request app.
/// </summary>
///
/// <param name="appHostResolver">The resolver used to determine the current request app.</param>
///
/// <author>Almighty-Shogun</author>
/// <since>Unreleased</since>
internal sealed class AppAudienceAuthorizationHandler(IAppHostResolver appHostResolver) : AuthorizationHandler<AppAudienceRequirement>
{
    /// <inheritdoc />
    protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, AppAudienceRequirement requirement)
    {
        if (!appHostResolver.TryResolve(out string? app))
            return Task.CompletedTask;

        if (app is null || HasAudience(context.User, app))
            context.Succeed(requirement);

        return Task.CompletedTask;
    }

    /// <summary>
    /// Checks whether the authenticated principal contains the expected audience claim.
    /// </summary>
    ///
    /// <param name="principal">The authenticated principal to inspect.</param>
    /// <param name="app">The expected app audience value.</param>
    ///
    /// <returns><c>true</c> when the expected audience exists; otherwise, <c>false</c>.</returns>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    private static bool HasAudience(ClaimsPrincipal principal, string app) => principal.Claims
        .Where(IsAudienceClaim)
        .Any(claim => string.Equals(claim.Value, app, StringComparison.OrdinalIgnoreCase));

    /// <summary>
    /// Determines whether a claim represents a JWT audience value.
    /// </summary>
    ///
    /// <param name="claim">The claim to inspect.</param>
    ///
    /// <returns><c>true</c> when the claim type is a supported audience claim type; otherwise, <c>false</c>.</returns>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    private static bool IsAudienceClaim(Claim claim)
        => claim.Type is JwtRegisteredClaimNames.Aud or "http://schemas.microsoft.com/identity/claims/audience";
}
