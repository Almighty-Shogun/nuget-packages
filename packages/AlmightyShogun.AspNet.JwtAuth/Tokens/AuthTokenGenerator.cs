using System.Security.Claims;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Microsoft.IdentityModel.JsonWebTokens;

namespace AlmightyShogun.AspNet.JwtAuth;

/// <summary>
/// Mints signed access tokens. The signing credentials are built once and reused, because deriving them per token would
/// repeat the secret's byte encoding and its length check on every sign-in.
/// </summary>
///
/// <param name="authOptions">
/// The bound settings supplying the issuer, the signing secret, and how long a minted token stays valid.
/// </param>
/// <param name="appHostResolver">The resolver used to determine the audience when the caller does not supply one.</param>
///
/// <author>Almighty-Shogun</author>
/// <since>Unreleased</since>
internal sealed class AuthTokenGenerator(IOptions<AuthSettings> authOptions, IAppHostResolver appHostResolver) : IAuthTokenGenerator
{
    /// <summary>
    /// The token handler. Creating one is not free, and it is thread-safe, so a single instance is shared.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    private static readonly JsonWebTokenHandler _tokenHandler = new();

    /// <summary>
    /// The signing credentials, derived once from the configured secret. Deriving them per call would repeat the key
    /// validation and the byte encoding on every token minted.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    private readonly SigningCredentials _signingCredentials = new(
        authOptions.Value.SigningKey(),
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

        return new AuthToken(_tokenHandler.CreateToken(descriptor), expiresAt);
    }
}
