# Installation

Install `AlmightyShogun.AspNet.CredentialAuth` in the ASP.NET Core API that owns credential users, sessions, and password reset tokens. The application supplies an EF Core context derived from [`AuthDbContext<TUser>`](./types/auth-db-context), so credential data lives in the same database and the same migrations as the rest of the domain.

```sh
dotnet add package AlmightyShogun.AspNet.CredentialAuth
```

## Dependencies

### Framework references

- `Microsoft.AspNetCore.App` &mdash; supplies the HTTP abstractions, dependency injection, Identity password hashing, data protection, and WebUtilities APIs the package builds on.

### Package references

- `Microsoft.EntityFrameworkCore` `10.0.11` &mdash; supplies the base `DbContext`, model building, and query APIs used for users, sessions, tokens, and two-factor enrolments.
- `Otp.NET` `1.4.1` &mdash; generates and verifies the TOTP codes behind [`IAuthTwoFactorService<TUser>`](./services/auth-two-factor-service).

### Project references

- `AlmightyShogun.AspNet.JwtAuth` &mdash; supplies [`AuthSettings`](/asp-net-jwt-auth/configuration), [`IAppHostResolver`](/asp-net-jwt-auth/services/app-host-resolver), the token generator, and the refresh-token cookie helpers.
- `AlmightyShogun.AspNet.Core` &mdash; supplies [`SessionContext`](/asp-net-core/records/session-context), User-Agent parsing, and the [`IExceptionMapper`](/asp-net-core/exceptions) contract this package's exceptions map through.
- `AlmightyShogun.AspNet.Localization` &mdash; resolves the message on each failure from the `auth` and `passwords` files described in [Localization](./localization).
- `AlmightyShogun.AspNet.RequestValidation` &mdash; supplies the `[Required]`, `[Email]`, `[Min]`, and `[PasswordSecure]` rules carried by the [request models](./requests/login-request).

## Startup Registration

[`AddJwtAuth`](/asp-net-jwt-auth/extensions/add-jwt-auth) binds the token settings and the app resolver that credential flows read; [`AddCredentialAuth`](./extensions/add-credential-auth) maps your context onto the package base context and registers the credential services against your user entity.

::: warning
Register JWT auth first. Credential auth resolves [`AuthSettings`](/asp-net-jwt-auth/configuration) and [`IAppHostResolver`](/asp-net-jwt-auth/services/app-host-resolver) at construction, so a container missing them fails when the first credential service is resolved, not at startup.
:::

::: code-group

```csharp [Program.cs]
using AlmightyShogun.AspNet.Core;
using Microsoft.EntityFrameworkCore;
using AlmightyShogun.AspNet.JwtAuth;
using AlmightyShogun.AspNet.Localization;
using AlmightyShogun.AspNet.CredentialAuth;
using AlmightyShogun.AspNet.RequestValidation;

builder.Services
    .AddMessageLocalization(builder.Configuration)
    .AddHttpErrorResponseWriter()
    .AddExceptionHandling()
    .AddJwtAuth(builder.Configuration)
    .AddAspNetValidation()
    .AddDbContext<AppDbContext>(options => ...)
    .AddCredentialAuth<AppDbContext, AppUser>(builder.Configuration);

WebApplication app = builder.Build();

app.UseHttpErrorResponses();
app.UseAspNetValidation();
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

public sealed class AppUser : AuthUser;
```

:::

## Data protection keys

Two-factor secrets are encrypted with ASP.NET Core data protection before they are stored. The default key ring lives on the local machine, so an application running on more than one host, or in a container without a persisted key directory, must configure a shared key store before anyone enrols. Losing the keys makes every stored secret unreadable and forces every enrolled user to set up their authenticator again.
