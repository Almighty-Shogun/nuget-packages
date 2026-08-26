using System.Security.Claims;
using Microsoft.Extensions.Options;
using AlmightyShogun.AspNet.JwtAuth;

namespace AlmightyShogun.AspNet.CredentialAuth;

/// <summary>
/// Builds the claim set for a user and hands it to the JWT package to sign, so this package decides what a token says
/// and the other decides how it is proved.
/// </summary>
///
/// <typeparam name="TUser">The application's own user entity, read for the claims a token carries.</typeparam>
/// <param name="databaseContext">The application's context, so auth writes join whatever transaction it is in.</param>
/// <param name="authOptions">The bound JWT settings, read for token and session lifetimes.</param>
/// <param name="appHostResolver">
/// The resolver deciding which application the current request belongs to, so what is issued is scoped to it.
/// </param>
/// <param name="tokenGenerator">
/// The JWT package's generator, which signs and stamps issuer, audience, and expiry, so this service only decides the
/// claims.
/// </param>
///
/// <author>Almighty-Shogun</author>
/// <since>Unreleased</since>
internal sealed class AuthTokenService<TUser>(
    AuthDbContext<TUser> databaseContext,
    IOptions<AuthSettings> authOptions,
    IAppHostResolver appHostResolver,
    IAuthTokenGenerator tokenGenerator
) : AuthServiceBase<TUser>(databaseContext, authOptions, appHostResolver), IAuthTokenService<TUser> where TUser : AuthUser
{
    /// <inheritdoc />
    public string GenerateToken(TUser user, string? app = null)
    {
        List<Claim> claims =
        [
            new(AuthClaimTypes.UserId, user.Identifier.ToString()),
            new("username", user.Username),
            new(ClaimTypes.NameIdentifier, user.Identifier.ToString()),
            new(ClaimTypes.Role, user.Role)
        ];

        IEnumerable<string> permissions = app is null
            ? user.Permissions
            : user.Permissions
                .Where(permission => permission.StartsWith($"{app}:", StringComparison.OrdinalIgnoreCase))
                .Select(permission => permission[(app.Length + 1)..]);

        claims.AddRange(permissions.Select(permission => new Claim(AuthClaimTypes.Permission, permission)));

        return tokenGenerator.Generate(claims, app).Token;
    }
}
