# ASP.NET Credential Auth

Adds username or email and password authentication to ASP.NET Core APIs, storing users, refresh-token sessions, password reset tokens, and TOTP enrolments in the application's own Entity Framework Core context.

Use this package when an API needs first-party accounts instead of an external provider such as Discord or Google. It owns the credential side of that: verifying passwords, rotating refresh sessions, locking accounts after repeated failures, running the forgot-password flow, and enrolling a second factor. Issuing and validating the access token itself is left to [ASP.NET JWT Auth](/asp-net-jwt-auth/), which this package builds on.

Application code depends on small service contracts rather than one aggregate, so a controller only takes the part of the surface it uses. Every credential failure surfaces as a typed exception that maps to a standardized HTTP error through [ASP.NET Core](/asp-net-core/exceptions), with the text resolved per language.

## Categories

- [Configuration](./configuration) &mdash; session lifetime ceiling, password reset lifetime, lockout policy, and two-factor policy.
- [Exceptions](./exceptions) &mdash; every credential failure, with the status code and `error` value it becomes.
- [Localization](./localization) &mdash; the `auth` and `passwords` message files those failures resolve through.
- [Extensions](./extensions/add-credential-auth) &mdash; service registration against the application's context and user entity.
- [Services](./services/auth-user-service) &mdash; dependency-injection contracts for login, session, password, token, and two-factor operations.
- [Requests](./requests/login-request) &mdash; request models for login, registration, and password flows.
- [Results](./results/auth-session-result) &mdash; what a credential flow returns when it creates a session or an enrolment.
- [Utilities](./utilities/token-hasher) &mdash; the digest every stored token is matched by.
- [Types](./types/auth-db-context) &mdash; the base context and the entities it maps.

## Quick Example

::: code-group

```csharp [Program.cs]
using AlmightyShogun.AspNet.Core;
using Microsoft.EntityFrameworkCore;
using AlmightyShogun.AspNet.JwtAuth;
using AlmightyShogun.AspNet.CredentialAuth;
using AlmightyShogun.AspNet.RequestValidation;

builder.Services
    .AddHttpErrorResponses(builder.Configuration)
    .AddJwtAuth(builder.Configuration)
    .AddAspNetValidation()
    .AddDbContext<AppDbContext>(options => ...)
    .AddCredentialAuth<AppDbContext, AppUser>(builder.Configuration);
```

```csharp [AuthController.cs]
using Microsoft.AspNetCore.Mvc;
using AlmightyShogun.AspNet.JwtAuth;
using AlmightyShogun.AspNet.CredentialAuth;

[ApiController]
[Route("auth")]
public sealed class AuthController(
    IAuthUserService<AppUser> authUsers
) : ControllerBase
{
    [HttpPost("login")]
    public async Task<ActionResult<AppUser>> Login(LoginRequest request)
    {
        AuthSessionResult<AppUser> result = await authUsers
            .LoginAsync(request, HttpContext);

        Response.SetRefreshTokenCookie(result.RefreshToken, 30);

        return Ok(result.User);
    }
}
```

```csharp [AppDbContext.cs]
using Microsoft.EntityFrameworkCore;
using AlmightyShogun.AspNet.CredentialAuth;

public sealed class AppDbContext(
    DbContextOptions<AppDbContext> options
) : AuthDbContext<AppUser>(options);
```

```csharp [AppUser.cs]
using AlmightyShogun.AspNet.CredentialAuth;

public sealed class AppUser : AuthUser
{
    public string DisplayName { get; set; } = string.Empty;
}
```

:::
