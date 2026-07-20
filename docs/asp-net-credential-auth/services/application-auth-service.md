# ApplicationAuthService

Credential Auth intentionally registers focused service contracts such as [`IAuthUserService<TUser>`](./auth-user-service), [`IAuthSessionService<TUser>`](./auth-session-service), [`IAuthTokenService<TUser>`](./auth-token-service), and [`IAuthPasswordService`](./auth-password-service). This keeps the package generic and avoids exposing one aggregate interface for every possible user type.

When an application repeatedly needs the full auth surface, create an application-owned `IAuthService` facade. The facade can inherit the package interfaces with the concrete application user type, so controllers can depend on `IAuthService` without repeating `<AppUser>` everywhere.

::: code-group

```csharp [IAuthService.cs]
using AlmightyShogun.AspNet.CredentialAuth;

public interface IAuthService :
    IAuthUserService<AppUser>,
    IAuthSessionService<AppUser>,
    IAuthTokenService<AppUser>,
    IAuthPasswordService;
```

```csharp [AuthService.cs]
using Microsoft.AspNetCore.Http;
using AlmightyShogun.AspNet.CredentialAuth;

public sealed class AuthService(
    IAuthPasswordService passwords,
    IAuthTokenService<AppUser> tokens,
    IAuthUserService<AppUser> users,
    IAuthSessionService<AppUser> sessions) : IAuthService
{
    public Task<AuthSessionResult<AppUser>> LoginAsync(LoginRequest request, HttpContext context)
        => users.LoginAsync(request, context);

    public Task<AppUser> CreateUserAsync(AppUser user, string password)
        => users.CreateUserAsync(user, password);

    public Task<AuthSessionResult<AppUser>> RegisterAsync(
        AppUser user,
        string password,
        HttpContext context)
        => users.RegisterAsync(user, password, context);

    public Task<AuthSessionResult<AppUser>> RefreshSessionAsync(string refreshToken, HttpContext httpContext)
        => sessions.RefreshSessionAsync(refreshToken, httpContext);

    public string GenerateToken(AppUser user, string? app = null)
        => tokens.GenerateToken(user, app);

    public Task ChangePasswordAsync(
        int userId,
        ChangePasswordRequest request,
        string? currentRefreshToken = null)
        => passwords.ChangePasswordAsync(userId, request, currentRefreshToken);

    public Task<string> RequestForgotPasswordAsync(ForgotPasswordRequest request, string? requestIpAddress = null)
        => passwords.RequestForgotPasswordAsync(request, requestIpAddress);

    public Task CompleteForgotPasswordAsync(CompleteForgotPasswordRequest request)
        => passwords.CompleteForgotPasswordAsync(request);
}
```

```csharp [Program.cs]
builder.Services.AddScoped<IAuthService, AuthService>();
```

:::

Use this pattern only when it makes the application code clearer. Smaller controllers usually read better when they depend on the focused package service they actually use.
