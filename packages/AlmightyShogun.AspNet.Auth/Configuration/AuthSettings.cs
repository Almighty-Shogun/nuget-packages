using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace AlmightyShogun.AspNet.Auth;

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
    /// The issuer stamped into minted tokens and demanded of incoming ones, which is what stops a token from another
    /// system being accepted here.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>2.3.0</since>
    [Required]
    public required string Issuer { get; init; }

    /// <summary>
    /// The symmetric signing secret used to sign and validate JWT signatures. The <c>[MinLength]</c> constraint on it
    /// demands at least 32 characters.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>2.3.0</since>
    [Required]
    [MinLength(AuthSigningKey.MinimumSecretBytes)]
    public required string Secret { get; init; }

    /// <summary>
    /// How long a minted access token stays valid, in minutes. Kept short, because an access token cannot be
    /// revoked once issued: shortening it is the only thing that limits how long a leaked one is useful.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>2.3.0</since>
    [Range(1, int.MaxValue)]
    public int AccessTokenMinutes { get; init; } = 60;

    /// <summary>
    /// How long a refresh token stays valid, in days, which decides how long a returning user stays signed in
    /// without re-entering credentials.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>2.3.0</since>
    [Range(1, int.MaxValue)]
    public int RefreshTokenDays { get; init; } = 30;

    /// <summary>
    /// The tolerance applied when checking token expiry, in seconds, which absorbs small clock differences between
    /// the machine that minted a token and the machine validating it.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>4.0.0</since>
    [Range(1, int.MaxValue)]
    public int ClockSkewSeconds { get; init; } = 30;

    /// <summary>
    /// The audience used when no host mapping applies. Required when <see cref="Hosts"/> is empty.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>4.0.0</since>
    public string? DefaultApp { get; init; }

    /// <summary>
    /// The <c>SameSite</c> mode written on the refresh token cookie, read by <see cref="HttpResponseExtensions"/>.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>4.0.0</since>
    public SameSiteMode SameSite { get; init; } = SameSiteMode.Lax;

    /// <summary>
    /// The application audience name used when a request arrives from a loopback host and no host mapping matched it.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>2.3.0</since>
    public string? LocalhostApp { get; init; }

    /// <summary>
    /// The host-to-application mapping used for request host based audience validation.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>2.3.0</since>
    public IReadOnlyDictionary<string, string> Hosts { get; init; } = new Dictionary<string, string>();

    /// <summary>
    /// Every audience a token may carry: the host mappings, the localhost fallback, and the default app. It never
    /// returns empty: every path either adds an audience or throws.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>4.0.0</since>
    public IReadOnlyList<string> ValidAudiences => _validAudiences ??= BuildValidAudiences();

    /// <summary>
    /// The cached audience list. Building it walks the host mapping and can throw, so it is built the first time
    /// <see cref="ValidAudiences"/> is read on an instance and reused for every later read of that instance. Startup
    /// validation and the bearer options need not share one, so it may be built more than once in a process.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>4.0.0</since>
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
    /// <since>4.0.0</since>
    public bool IsScoped() => Hosts.Count > 0;

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
    /// <since>4.0.0</since>
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
