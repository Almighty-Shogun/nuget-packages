using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.AspNetCore.Authorization;

namespace AlmightyShogun.AspNet.JwtAuth;

/// <summary>
/// Authorizes requests by ensuring the current token audience matches the resolved request app.
/// </summary>
///
/// <param name="appHostResolver">
/// The resolver deciding which application the current request belongs to, so this handler compares the token's audience
/// against the host rather than against a fixed value.
/// </param>
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
    /// Reports whether the principal carries the audience the request host resolved to, which is what stops a token minted
    /// for one application being replayed against another.
    /// </summary>
    ///
    /// <param name="principal">The authenticated caller, whose audience claims are being checked.</param>
    /// <param name="app">The audience the request host resolved to, compared case-insensitively.</param>
    ///
    /// <returns>
    /// <c>true</c> when the principal carries that audience. A token may carry several, so one match is enough.
    /// </returns>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    private static bool HasAudience(ClaimsPrincipal principal, string app) => principal.Claims
        .Where(IsAudienceClaim)
        .Any(claim => string.Equals(claim.Value, app, StringComparison.OrdinalIgnoreCase));

    /// <summary>
    /// Reports whether a claim is an audience, accepting both the short JWT name and the long URI form, because which one
    /// appears depends on whether inbound claim mapping was left on.
    /// </summary>
    ///
    /// <param name="claim">One claim from the principal.</param>
    ///
    /// <returns><c>true</c> when the claim type is a supported audience claim type; otherwise, <c>false</c>.</returns>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    private static bool IsAudienceClaim(Claim claim)
        => claim.Type is JwtRegisteredClaimNames.Aud or "http://schemas.microsoft.com/identity/claims/audience";
}
