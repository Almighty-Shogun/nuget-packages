using System.Text;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;

namespace AlmightyShogun.AspNet.CredentialAuth;

/// <summary>
/// Generates JWT access tokens for the authentication service.
/// </summary>
/// <author>Almighty-Shogun</author>
/// <since>Unreleased</since>
internal sealed partial class AuthService<TUser> where TUser : AuthUser
{
    /// <inheritdoc />
    public string GenerateToken(TUser user, string? app = null)
    {
        List<Claim> claims =
        [
            new("userId", user.Id.ToString()),
            new("username", user.Username),
            new(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new(ClaimTypes.Role, user.Role)
        ];

        if (app is not null)
        {
            claims.AddRange(user.Permissions
                .Where(p => p.StartsWith($"{app}:", StringComparison.OrdinalIgnoreCase))
                .Select(p => new Claim("permission", p)));
        }
        else
        {
            claims.AddRange(user.Permissions.Select(p => new Claim("permission", p)));
        }

        SymmetricSecurityKey key = new(Encoding.UTF8.GetBytes(AuthSettings.Secret));

        JwtSecurityToken token = new(
            claims: claims,
            issuer: AuthSettings.Issuer,
            audience: app ?? string.Empty,
            expires: DateTime.UtcNow.AddHours(AuthSettings.Hours),
            signingCredentials: new SigningCredentials(key, SecurityAlgorithms.HmacSha256)
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}
