# Controllers

Credential Auth provides services, request DTOs, and EF Core auth entities, not ready-made controller classes. The examples below show a practical route shape for login, registration, refresh-token rotation, logout, forgot-password flows, and session management using the public package APIs.

These examples assume the application has already registered [ASP.NET Utils](/asp-net-utils/), [ASP.NET JWT Auth](/asp-net-jwt-auth/), [ASP.NET Validation](/asp-net-validation/), the application [`AuthDbContext<TUser>`](./types/auth-db-context), and Credential Auth as shown on [Installation](./installation). Validation failures and service exceptions are handled by the ASP.NET Validation and HTTP error middleware.

## Auth Controller

Use this controller for login, registration, refresh-token rotation, and browser logout. Login calls [`LoginAsync`](./services/auth-user-service#loginasync), registration calls [`RegisterAsync`](./services/auth-user-service#registerasync), and refresh calls [`RefreshSessionAsync`](./services/auth-session-service#refreshsessionasync).

The refresh-token helpers come from ASP.NET JWT Auth: [`RequireRefreshToken`](/asp-net-jwt-auth/attributes/require-refresh-token-attribute), [`GetRefreshTokenCookie`](/asp-net-jwt-auth/extensions/get-refresh-token-cookie), [`SetRefreshTokenCookie`](/asp-net-jwt-auth/extensions/set-refresh-token-cookie), and [`DeleteAuthCookies`](/asp-net-jwt-auth/extensions/delete-auth-cookies). Logout uses the application `AppDbContext` directly to revoke the current refresh-token session before deleting the browser cookie.

::: code-group

```csharp [AuthController.cs]
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.EntityFrameworkCore;
using AlmightyShogun.AspNet.JwtAuth;
using AlmightyShogun.AspNet.CredentialAuth;

[ApiController]
[Route("auth")]
public sealed class AuthController(
    IAuthUserService<AppUser> userService,
    AppDbContext databaseContext,
    IAuthSessionService<AppUser> sessionService,
    IOptions<AuthSettings> authOptions) : ControllerBase
{
    private readonly AuthSettings _authSettings = authOptions.Value;

    [HttpPost("login")]
    public async Task<IActionResult> LoginAsync([FromBody] LoginRequest request)
    {
        AuthSessionResult<AppUser> result = await userService.LoginAsync(request, HttpContext);

        SetRefreshTokenCookie(result);

        return Ok(new
        {
            result.AccessToken,
            result.User
        });
    }

    [HttpPost("register")]
    public async Task<IActionResult> RegisterAsync([FromBody] CreateUserRequest request)
    {
        AppUser user = new()
        {
            Role = request.Role,
            Email = request.Email,
            Username = request.Username,
            Permissions = request.Permissions
        };

        AuthSessionResult<AppUser> result = await userService.RegisterAsync(user, request.Password, HttpContext);

        SetRefreshTokenCookie(result);

        return Ok(new
        {
            result.AccessToken,
            result.User
        });
    }

    [HttpPost("refresh")]
    [RequireRefreshToken]
    public async Task<IActionResult> RefreshAsync()
    {
        string refreshToken = Request.GetRefreshTokenCookie();
        AuthSessionResult<AppUser> result = await sessionService.RefreshSessionAsync(refreshToken, HttpContext);

        SetRefreshTokenCookie(result);

        return Ok(new
        {
            result.AccessToken,
            result.User
        });
    }

    [HttpPost("logout")]
    [RequireRefreshToken]
    public async Task<IActionResult> LogoutAsync()
    {
        string refreshToken = Request.GetRefreshTokenCookie();

        UserSession? session = await databaseContext.UserSessions
            .FirstOrDefaultAsync(session => session.RefreshToken == refreshToken);

        if (session is not null)
        {
            session.IsRevoked = true;
            await databaseContext.SaveChangesAsync();
        }

        Response.DeleteAuthCookies();

        return NoContent();
    }

    private void SetRefreshTokenCookie(AuthSessionResult<AppUser> result)
        => Response.SetRefreshTokenCookie(result.RefreshToken, _authSettings.RefreshTokenDays);
}
```

:::

If a controller needs several auth services and the generic type gets repetitive, create an application-owned facade as shown on [`ApplicationAuthService`](./services/application-auth-service).

## Forgot Password Controller

Use this controller for password-reset requests, token checks, and reset completion. Credential Auth creates and validates the reset token through [`RequestForgotPasswordAsync`](./services/auth-password-service#requestforgotpasswordasync), [`ForgotPasswordTokenRequest`](./requests/forgot-password-token-request), and [`CompleteForgotPasswordAsync`](./services/auth-password-service#completeforgotpasswordasync). The application still owns email delivery.

::: code-group

```csharp [ForgotPasswordController.cs]
using Microsoft.AspNetCore.Mvc;
using AlmightyShogun.AspNet.CredentialAuth;

[ApiController]
[Route("auth/forgot-password")]
public sealed class ForgotPasswordController(
    IAuthPasswordService passwordService,
    IPasswordResetMailer passwordResetMailer) : ControllerBase
{
    [HttpPost]
    public async Task<IActionResult> ForgotPasswordAsync([FromBody] ForgotPasswordRequest request)
    {
        string? ipAddress = HttpContext.Connection.RemoteIpAddress?.ToString();
        string token = await passwordService.RequestForgotPasswordAsync(request, ipAddress);

        await passwordResetMailer.SendResetLinkAsync(request.Email, token);

        return NoContent();
    }

    [HttpPost("token")]
    public IActionResult CheckToken([FromBody] ForgotPasswordTokenRequest request)
        => NoContent();

    [HttpPost("complete")]
    public async Task<IActionResult> CompleteForgotPasswordAsync([FromBody] CompleteForgotPasswordRequest request)
    {
        await passwordService.CompleteForgotPasswordAsync(request);

        return NoContent();
    }
}
```

```csharp [IPasswordResetMailer.cs]
public interface IPasswordResetMailer
{
    Task SendResetLinkAsync(string email, string token);
}
```

:::

The `token` route intentionally does not call a service method. [`ForgotPasswordTokenRequest`](./requests/forgot-password-token-request) uses [`PasswordResetToken`](./attributes/password-reset-token-attribute), so a successful action means the submitted token passed validation.

## Session Controller

Use this controller when the API needs to list or revoke refresh-token sessions. Credential Auth exposes [`UserSession`](./types/user-session) and [`AuthDbContext<TUser>.UserSessions`](./types/auth-db-context), so the controller can use the registered auth database context directly instead of a custom repository.

::: code-group

```csharp [SessionController.cs]
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AlmightyShogun.AspNet.JwtAuth;
using Microsoft.AspNetCore.Authorization;
using AlmightyShogun.AspNet.CredentialAuth;

[ApiController]
[Authorize]
[Route("auth/sessions")]
public sealed class SessionController(AppDbContext databaseContext) : ControllerBase
{
    [HttpGet]
    [RequireRefreshToken]
    public async Task<ActionResult<List<UserSessionResponse>>> GetSessionsAsync()
    {
        int userId = User.GetCurrentUserId();
        string currentToken = Request.GetRefreshTokenCookie();

        List<UserSession> sessions = await databaseContext.UserSessions
            .Where(session => session.UserId == userId)
            .OrderByDescending(session => session.LastActiveAt)
            .ToListAsync();

        List<UserSessionResponse> response = sessions
            .Select(session => UserSessionResponse.From(session, session.RefreshToken == currentToken))
            .ToList();

        return Ok(response);
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> RevokeSessionAsync(int id)
    {
        int userId = User.GetCurrentUserId();

        UserSession? session = await databaseContext.UserSessions
            .FirstOrDefaultAsync(session => session.Id == id && session.UserId == userId);

        if (session is null)
            return NotFound();

        session.IsRevoked = true;
        await databaseContext.SaveChangesAsync();

        return NoContent();
    }

    [HttpDelete]
    public async Task<IActionResult> RevokeAllSessionsAsync()
    {
        int userId = User.GetCurrentUserId();

        List<UserSession> sessions = await databaseContext.UserSessions
            .Where(session => session.UserId == userId && !session.IsRevoked)
            .ToListAsync();

        foreach (UserSession session in sessions)
            session.IsRevoked = true;

        await databaseContext.SaveChangesAsync();

        Response.DeleteAuthCookies();

        return NoContent();
    }
}
```

```csharp [UserSessionResponse.cs]
using AlmightyShogun.AspNet.CredentialAuth;

public sealed record UserSessionResponse(
    int Id,
    string App,
    string? Device,
    string? Browser,
    string? IpAddress,
    string? UserAgent,
    DateTime ExpiresAt,
    DateTime CreatedAt,
    DateTime LastActiveAt,
    bool IsRevoked,
    bool IsCurrent)
{
    public static UserSessionResponse From(UserSession session, bool isCurrent)
        => new(
            session.Id,
            session.App,
            session.Device,
            session.Browser,
            session.IpAddress,
            session.UserAgent,
            session.ExpiresAt,
            session.CreatedAt,
            session.LastActiveAt,
            session.IsRevoked,
            isCurrent);
}
```

:::

The session routes use [`GetCurrentUserId`](/asp-net-jwt-auth/extensions/get-current-user-id) so users can only query and revoke their own sessions. The session list route uses [`RequireRefreshToken`](/asp-net-jwt-auth/attributes/require-refresh-token-attribute) and [`GetRefreshTokenCookie`](/asp-net-jwt-auth/extensions/get-refresh-token-cookie) so the current browser session can be marked without nullable refresh-token handling.
