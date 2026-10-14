
using System.Security.Claims;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Microsoft.IdentityModel.JsonWebTokens;

namespace AlmightyShogun.AspNet.Auth;

/// <summary>
/// Generates signed JWT access tokens.
/// </summary>
/// <param name="authOptions">The authentication settings.</param>
/// <param name="appHostResolver">The application host resolver.</param>
/// <author>Almighty-Shogun</author>
/// <since>4.0.0</since>
internal sealed class AuthTokenGenerator(IOptions<AuthSettings> authOptions, IAppHostResolver appHostResolver) : IAuthTokenGenerator
{
    /// <summary>
    /// The handler used to generate JWTs.
    /// </summary>
    /// <author>Almighty-Shogun</author>
    /// <since>4.0.0</since>
    private static readonly JsonWebTokenHandler _tokenHandler = new();

    /// <summary>
    /// The credentials used to sign tokens.
    /// </summary>
    /// <author>Almighty-Shogun</author>
    /// <since>4.0.0</since>
    private readonly SigningCredentials _signingCredentials = new(
        AuthSigningKey.Create(authOptions.Value.Secret),
        SecurityAlgorithms.HmacSha256
    );

    /// <inheritdoc />
    public AuthToken Generate(IEnumerable<Claim> claims, string? audience = null)
    {
        AuthSettings settings = authOptions.Value;
        DateTime expiresAt = DateTime.UtcNow.AddMinutes(settings.AccessTokenMinutes);

        string resolvedAudience = audience ?? (settings.IsScoped() ? appHostResolver.Resolve() : null)
            ?? settings.DefaultApp
            ?? throw new InvalidOperationException(
                "No audience could be resolved. Configure Auth:DefaultApp or a matching Auth:Hosts entry."
            );

        SecurityTokenDescriptor descriptor = new()
        {
            Issuer = settings.Issuer,
            Audience = resolvedAudience,
            Expires = expiresAt,
            SigningCredentials = _signingCredentials,
            Subject = new ClaimsIdentity(claims)
        };

        return new AuthToken
        {
            Token = _tokenHandler.CreateToken(descriptor),
            ExpiresAt = expiresAt
        };
    }
}
