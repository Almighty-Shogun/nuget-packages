using Microsoft.AspNetCore.Authorization;

namespace AlmightyShogun.AspNet.JwtAuth;

/// <summary>
/// Requires the authenticated token audience to match the request app when app scoping is active.
/// </summary>
///
/// <author>Almighty-Shogun</author>
/// <since>Unreleased</since>
internal sealed class AppAudienceRequirement : IAuthorizationRequirement;
