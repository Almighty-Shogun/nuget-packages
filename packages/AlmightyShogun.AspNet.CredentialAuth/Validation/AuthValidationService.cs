using Microsoft.AspNetCore.Http;
using AlmightyShogun.AspNet.JwtAuth;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace AlmightyShogun.AspNet.CredentialAuth;

/// <summary>
/// Provides database-backed validation checks for authentication requests.
/// </summary>
///
/// <param name="databaseContext">The authentication database context used for validation lookups.</param>
/// <param name="httpContextAccessor">The HTTP context accessor used to resolve the current authenticated user.</param>
///
/// <typeparam name="TUser">The authentication user entity type.</typeparam>
///
/// <author>Almighty-Shogun</author>
/// <since>Unreleased</since>
internal sealed class AuthValidationService<TUser>(
    AuthDbContext<TUser> databaseContext,
    IHttpContextAccessor httpContextAccessor) : IAuthValidationService where TUser : AuthUser
{
    /// <summary>
    /// The password hasher used to verify stored authentication passwords.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    private readonly PasswordHasher<TUser> _hasher = new();

    /// <summary>
    /// The identifier of the current authenticated user, or <c>0</c> when no user is available.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    private int CurrentUserId => httpContextAccessor.HttpContext?.User.GetCurrentUserId() ?? 0;

    /// <inheritdoc />
    public async Task<bool> IsCurrentPasswordAsync(string? currentPassword, CancellationToken cancellationToken = default)
    {
        if (currentPassword is null)
            return true;

        if (CurrentUserId <= 0)
            return false;

        TUser? user = await databaseContext.Users.FindAsync([CurrentUserId], cancellationToken);

        return user is not null && PasswordMatches(user, currentPassword);
    }

    /// <inheritdoc />
    public async Task<bool> IsDifferentFromCurrentPasswordAsync(string? newPassword, CancellationToken cancellationToken = default)
    {
        if (newPassword is null)
            return true;

        if (CurrentUserId <= 0)
            return false;

        TUser? user = await databaseContext.Users.FindAsync([CurrentUserId], cancellationToken);

        return user is not null && !PasswordMatches(user, newPassword);
    }

    /// <inheritdoc />
    public async Task<bool> LoginIdentifierExistsAsync(string? identifier, CancellationToken cancellationToken = default)
        => identifier is null
           || await databaseContext.Users.AnyAsync(user => user.Username == identifier || user.Email == identifier, cancellationToken);

    /// <inheritdoc />
    public async Task<bool> IsUsernameUniqueAsync(string? username, CancellationToken cancellationToken = default)
        => username is null
           || !await databaseContext.Users.AnyAsync(user => user.Username == username, cancellationToken);

    /// <inheritdoc />
    public async Task<bool> IsEmailUniqueAsync(string? email, CancellationToken cancellationToken = default)
        => email is null
           || !await databaseContext.Users.AnyAsync(user => user.Email == email, cancellationToken);

    /// <inheritdoc />
    public async Task<bool> IsCurrentPasswordAsync(
        string? identifier,
        string? currentPassword,
        CancellationToken cancellationToken = default)
    {
        if (identifier is null || currentPassword is null)
            return true;

        TUser? user = await FindUserByIdentifierAsync(identifier, cancellationToken);

        return user is not null && PasswordMatches(user, currentPassword);
    }

    /// <inheritdoc />
    public async Task<bool> IsPasswordResetTokenActiveAsync(string? token, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrEmpty(token))
            return true;

        string tokenHash = TokenHasher.Hash(token);

        return await databaseContext.PasswordResetTokens
            .Where(passwordToken => passwordToken.ExpiresAt > DateTime.UtcNow)
            .Where(passwordToken => passwordToken.UsedAt == null && passwordToken.TokenHash == tokenHash)
            .AnyAsync(cancellationToken);
    }

    /// <inheritdoc />
    public async Task<bool> IsDifferentFromPasswordResetTokenUserAsync(
        string? token,
        string? newPassword,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrEmpty(token) || newPassword is null)
            return true;

        string tokenHash = TokenHasher.Hash(token);

        PasswordResetToken? passwordToken = await databaseContext.PasswordResetTokens
            .Where(resetToken => resetToken.ExpiresAt > DateTime.UtcNow)
            .Where(resetToken => resetToken.UsedAt == null && resetToken.TokenHash == tokenHash)
            .FirstOrDefaultAsync(cancellationToken);

        if (passwordToken is null)
            return true;

        TUser? user = await databaseContext.Users.FindAsync([passwordToken.UserId], cancellationToken);

        return user is not null && !PasswordMatches(user, newPassword);
    }

    /// <summary>
    /// Finds a user by username or email address.
    /// </summary>
    ///
    /// <param name="identifier">The username or email address.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    ///
    /// <returns>The matching user when found; otherwise, <c>null</c>.</returns>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    private async Task<TUser?> FindUserByIdentifierAsync(string identifier, CancellationToken cancellationToken)
        => await databaseContext.Users
            .FirstOrDefaultAsync(user => user.Username == identifier || user.Email == identifier, cancellationToken);

    /// <summary>
    /// Checks whether a password matches the supplied authentication user.
    /// </summary>
    ///
    /// <param name="user">The authentication user.</param>
    /// <param name="password">The password value.</param>
    ///
    /// <returns><c>true</c> when the password matches; otherwise, <c>false</c>.</returns>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    private bool PasswordMatches(TUser user, string password)
        => _hasher.VerifyHashedPassword(user, user.Password, password) is not PasswordVerificationResult.Failed;
}
