using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using AlmightyShogun.AspNet.JwtAuth;

namespace AlmightyShogun.AspNet.CredentialAuth;

/// <summary>
/// Provides database-backed validation checks for authentication requests.
/// </summary>
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

    /// <inheritdoc />
    public async Task<bool> IsCurrentPasswordAsync(
        string? currentPassword,
        CancellationToken cancellationToken = default)
    {
        if (currentPassword is null)
            return true;

        int userId = httpContextAccessor.HttpContext?.User.GetCurrentUserId() ?? 0;

        if (userId <= 0)
            return false;

        TUser? user = await databaseContext.Users.FindAsync([userId], cancellationToken);

        return user is not null && PasswordMatches(user, currentPassword);
    }

    /// <inheritdoc />
    public async Task<bool> IsDifferentFromCurrentPasswordAsync(
        string? newPassword,
        CancellationToken cancellationToken = default)
    {
        if (newPassword is null)
            return true;

        int userId = httpContextAccessor.HttpContext?.User.GetCurrentUserId() ?? 0;

        if (userId <= 0)
            return false;

        TUser? user = await databaseContext.Users.FindAsync([userId], cancellationToken);

        return user is not null && !PasswordMatches(user, newPassword);
    }

    /// <inheritdoc />
    public async Task<bool> LoginIdentifierExistsAsync(
        string? identifier,
        CancellationToken cancellationToken = default)
    {
        if (identifier is null)
            return true;

        return await databaseContext.Users
            .AnyAsync(user => user.Username == identifier || user.Email == identifier, cancellationToken);
    }

    /// <inheritdoc />
    public async Task<bool> IsUsernameUniqueAsync(
        string? username,
        CancellationToken cancellationToken = default)
    {
        if (username is null)
            return true;

        return !await databaseContext.Users
            .AnyAsync(user => user.Username == username, cancellationToken);
    }

    /// <inheritdoc />
    public async Task<bool> IsEmailUniqueAsync(
        string? email,
        CancellationToken cancellationToken = default)
    {
        if (email is null)
            return true;

        return !await databaseContext.Users
            .AnyAsync(user => user.Email == email, cancellationToken);
    }

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
    public async Task<bool> IsPasswordResetTokenActiveAsync(
        string? token,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrEmpty(token))
            return true;

        string tokenHash = PasswordResetTokenHasher.Hash(token);

        return await databaseContext.PasswordResetTokens
            .AnyAsync(
                passwordToken => passwordToken.TokenHash == tokenHash
                                 && passwordToken.UsedAt == null
                                 && passwordToken.ExpiresAt > DateTime.UtcNow,
                cancellationToken);
    }

    /// <inheritdoc />
    public async Task<bool> IsDifferentFromPasswordResetTokenUserAsync(
        string? token,
        string? newPassword,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrEmpty(token) || newPassword is null)
            return true;

        string tokenHash = PasswordResetTokenHasher.Hash(token);

        PasswordResetToken? passwordToken = await databaseContext.PasswordResetTokens
            .FirstOrDefaultAsync(
                resetToken => resetToken.TokenHash == tokenHash
                              && resetToken.UsedAt == null
                              && resetToken.ExpiresAt > DateTime.UtcNow,
                cancellationToken);

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
    {
        return await databaseContext.Users
            .FirstOrDefaultAsync(user => user.Username == identifier || user.Email == identifier, cancellationToken);
    }

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
    {
        PasswordVerificationResult verificationResult = _hasher.VerifyHashedPassword(user, user.Password, password);

        return verificationResult is not PasswordVerificationResult.Failed;
    }
}
