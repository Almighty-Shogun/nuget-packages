using Microsoft.AspNetCore.Http;
using AlmightyShogun.AspNet.Utils;
using Microsoft.EntityFrameworkCore.Storage;

namespace AlmightyShogun.AspNet.CredentialAuth;

/// <summary>
/// Provides login, user creation, and registration operations for the authentication service.
/// </summary>
///
/// <author>Almighty-Shogun</author>
/// <since>Unreleased</since>
internal sealed partial class AuthService<TUser> where TUser : AuthUser
{
    /// <inheritdoc />
    public async Task<AuthSessionResult<TUser>> LoginAsync(LoginRequest request, HttpContext context)
    {
        string? app = ResolveApp();
        SessionContext sessionContext = context.GetSessionContext();

        TUser user = await GetUserAsync(
            user => user.Username == request.Identifier || user.Email == request.Identifier,
            "auth.failed");

        string refreshToken = await CreateSessionAsync(user, app, sessionContext);

        return new AuthSessionResult<TUser>
        {
            User = user,
            RefreshToken = refreshToken,
            AccessToken = GenerateToken(user, app)
        };
    }

    /// <inheritdoc />
    public async Task<TUser> CreateUserAsync(TUser user, string password)
    {
        user.Password = _hasher.HashPassword(user, password);

        await DatabaseContext.Users.AddAsync(user);
        await DatabaseContext.SaveChangesAsync();

        return user;
    }

    /// <inheritdoc />
    public async Task<AuthSessionResult<TUser>> RegisterAsync(TUser user, string password, HttpContext context)
    {
        await using IDbContextTransaction transaction = await DatabaseContext.Database.BeginTransactionAsync();

        string? app = ResolveApp();
        SessionContext sessionContext = context.GetSessionContext();

        TUser createdUser = await CreateUserAsync(user, password);
        string refreshToken = await CreateSessionAsync(createdUser, app, sessionContext);

        AuthSessionResult<TUser> result = new()
        {
            User = createdUser,
            RefreshToken = refreshToken,
            AccessToken = GenerateToken(createdUser, app)
        };

        await transaction.CommitAsync();

        return result;
    }
}
