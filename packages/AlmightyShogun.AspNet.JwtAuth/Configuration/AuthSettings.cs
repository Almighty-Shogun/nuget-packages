using System.Text;
using Microsoft.AspNetCore.Http;
using Microsoft.IdentityModel.Tokens;
using System.ComponentModel.DataAnnotations;

namespace AlmightyShogun.AspNet.JwtAuth;

/// <summary>
/// The bound <c>Auth</c> section. Validated while the host starts, so a missing issuer, a secret too short to sign with,
/// or a configuration that leaves tokens with no audience stops the application there rather than failing the first
/// request.
/// </summary>
///
/// <author>Almighty-Shogun</author>
/// <since>2.3.0</since>
public sealed record AuthSettings
{
    /// <summary>
    /// The shortest signing key HMAC-SHA256 accepts, in bytes. Anything shorter is refused by the algorithm itself, so
    /// it is rejected here rather than surfacing as a signing failure on the first request.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    internal const int MinimumSecretBytes = 32;

    /// <summary>
    /// Gets the issuer stamped into minted tokens and demanded of incoming ones, which is what stops a token from another
    /// system being accepted here.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>2.3.0</since>
    [Required]
    public required string Issuer { get; init; }

    /// <summary>
    /// Gets the symmetric signing secret used to sign and validate JWT signatures. Must be at least 32 characters, which
    /// is what startup validation enforces. UTF-8 never encodes a character to fewer than one byte, so that also satisfies
    /// the 32-byte minimum HMAC-SHA256 accepts.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>2.3.0</since>
    [Required]
    [MinLength(MinimumSecretBytes)]
    public required string Secret { get; init; }

    /// <summary>
    /// Gets how long a minted access token stays valid, in minutes. Kept short, because an access token cannot be
    /// revoked once issued: shortening it is the only thing that limits how long a leaked one is useful.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>2.3.0</since>
    [Range(1, int.MaxValue)]
    public int AccessTokenMinutes { get; init; } = 60;

    /// <summary>
    /// Gets how long a refresh token stays valid, in days, which decides how long a returning user stays signed in
    /// without re-entering credentials. Pass it to <c>SetRefreshTokenCookie</c> to give the cookie a matching lifetime;
    /// nothing here does that for you.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>2.3.0</since>
    [Range(1, int.MaxValue)]
    public int RefreshTokenDays { get; init; } = 30;

    /// <summary>
    /// Gets the tolerance applied when checking token expiry, in seconds, which absorbs small clock differences between
    /// the machine that minted a token and the machine validating it.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    [Range(1, int.MaxValue)]
    public int ClockSkewSeconds { get; init; } = 30;

    /// <summary>
    /// Gets the audience used when no host mapping applies. Required when <see cref="Hosts"/> is empty, because audience
    /// validation is always on.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    public string? DefaultApp { get; init; }

    /// <summary>
    /// Gets the <c>SameSite</c> mode intended for the refresh token cookie. Nothing in this package reads it:
    /// <c>SetRefreshTokenCookie</c> writes <see cref="SameSiteMode.Lax"/> whatever this says, so an application needing
    /// another mode has to write the cookie itself.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    public SameSiteMode SameSite { get; init; } = SameSiteMode.Lax;

    /// <summary>
    /// Gets the application audience name used when requests arrive from plain localhost in development.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>2.3.0</since>
    public string? LocalhostApp { get; init; }

    /// <summary>
    /// Gets the host-to-application mapping used for request host based audience validation.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>2.3.0</since>
    public IReadOnlyDictionary<string, string> Hosts { get; init; } = new Dictionary<string, string>();

    /// <summary>
    /// Gets every audience a token may carry: the host mappings, the localhost fallback, and the default app. Read
    /// during startup validation, so a configuration that yields none stops the host rather than the first request.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    public IReadOnlyList<string> ValidAudiences => _validAudiences ??= BuildValidAudiences();

    /// <summary>
    /// The cached audience list. Building it walks the host mapping, and it is read on every token validation, so it is
    /// built once by startup validation and reused from there.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    private IReadOnlyList<string>? _validAudiences;

    /// <summary>
    /// Determines whether host-based app scoping is active for authentication and authorization.
    /// </summary>
    ///
    /// <returns>
    /// <c>true</c> when host mappings exist, which turns on host-based audience checks; <c>false</c> when every token is
    /// validated against the default app instead.
    /// </returns>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    public bool IsScoped() => Hosts.Count > 0;

    /// <summary>
    /// Builds the signing key, checking the secret in bytes rather than characters, which is what the algorithm actually
    /// constrains and what <see cref="MinLengthAttribute"/> on a string cannot express.
    /// </summary>
    ///
    /// <returns>
    /// The symmetric key minted tokens are signed with. Validation builds its own from the same secret rather than
    /// calling this, so the byte check below guards signing alone.
    /// </returns>
    ///
    /// <exception cref="InvalidOperationException">
    /// The secret encodes to fewer than <see cref="MinimumSecretBytes"/> bytes, so HMAC-SHA256 would refuse it. Not
    /// reachable while startup validation is on, since <see cref="MinLengthAttribute"/> already demands that many
    /// characters and a character never encodes to less than a byte.
    /// </exception>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    internal SymmetricSecurityKey SigningKey()
    {
        byte[] key = Encoding.UTF8.GetBytes(Secret);

        return key.Length >= MinimumSecretBytes
            ? new SymmetricSecurityKey(key)
            : throw new InvalidOperationException(
                $"Auth:Secret must encode to at least {MinimumSecretBytes} bytes for HMAC-SHA256 signing, but is {key.Length}."
            );
    }

    /// <summary>
    /// Collects the distinct configured audiences, refusing a configuration that would leave a token with no audience to
    /// be validated against.
    /// </summary>
    ///
    /// <returns>
    /// Every audience a token may legitimately carry, de-duplicated case-insensitively: the host mappings, the localhost
    /// fallback, and the default app.
    /// </returns>
    ///
    /// <exception cref="InvalidOperationException">
    /// A host is mapped to a blank audience, or no host mapping exists and <see cref="DefaultApp"/> is unset. Audience
    /// validation is always on, so either would refuse every token at runtime instead of at startup.
    /// </exception>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    private IReadOnlyList<string> BuildValidAudiences()
    {
        foreach (KeyValuePair<string, string> host in Hosts.Where(host => string.IsNullOrWhiteSpace(host.Value)))
            throw new InvalidOperationException($"Auth:Hosts entry '{host.Key}' has no audience value.");

        if (!IsScoped() && string.IsNullOrWhiteSpace(DefaultApp))
            throw new InvalidOperationException(
                "Auth:DefaultApp must be set when Auth:Hosts is empty, because every token carries an audience and it is always validated."
            );

        HashSet<string> audiences = new(StringComparer.OrdinalIgnoreCase);

        if (!string.IsNullOrWhiteSpace(DefaultApp))
            audiences.Add(DefaultApp);

        if (!string.IsNullOrWhiteSpace(LocalhostApp))
            audiences.Add(LocalhostApp);

        foreach (string audience in Hosts.Values.Where(audience => !string.IsNullOrWhiteSpace(audience)))
            audiences.Add(audience);

        return [.. audiences];
    }
}
