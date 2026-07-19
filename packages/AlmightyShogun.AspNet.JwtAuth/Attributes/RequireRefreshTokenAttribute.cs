using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace AlmightyShogun.AspNet.JwtAuth;

/// <summary>
/// Requires the current request to contain the package refresh-token cookie.
/// </summary>
///
/// <author>Almighty-Shogun</author>
/// <since>Unreleased</since>
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
public sealed class RequireRefreshTokenAttribute : Attribute, IAuthorizationFilter
{
    /// <inheritdoc />
    public void OnAuthorization(AuthorizationFilterContext context)
    {
        if (context.HttpContext.Request.TryGetRefreshTokenCookie() is null)
            context.Result = new UnauthorizedResult();
    }
}
