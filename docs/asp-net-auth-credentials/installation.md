# Installation

Install `AlmightyShogun.AspNet.Auth.Credentials` in the ASP.NET Core API that owns credential users, sessions, and password reset tokens. The application supplies an EF Core context derived from [`AuthDbContext<TUser>`](./types/auth-db-context), so credential data lives in the same database and the same migrations as the rest of the domain.

```sh
dotnet add package AlmightyShogun.AspNet.Auth.Credentials
```

## Dependencies

### Framework references

- `Microsoft.AspNetCore.App` &mdash; supplies the HTTP abstractions, dependency injection, Identity password hashing, data protection, and WebUtilities APIs the package builds on.

### Package references

- `Microsoft.EntityFrameworkCore` `10.0.11` &mdash; supplies the base `DbContext`, model building, and query APIs used for users, sessions, tokens, and two-factor enrolments.
- `Microsoft.EntityFrameworkCore.Relational` `10.0.11` &mdash; supplies the explicit transactions the credential flows run in, including the serializable ones that make token issuance and lockout counting safe under concurrency. It arrives with any relational provider anyway.
- `Otp.NET` `1.4.1` &mdash; generates and verifies the TOTP codes behind [`IAuthTwoFactorService<TUser>`](./services/auth-two-factor-service).

### Project references

- `AlmightyShogun.AspNet.Auth` &mdash; supplies [`AuthSettings`](/asp-net-auth/configuration), [`IAppHostResolver`](/asp-net-auth/services/app-host-resolver), the token generator, and the refresh-token cookie helpers.
- `AlmightyShogun.AspNet.Core` &mdash; supplies [`ClientContext`](/asp-net-core/records/client-context), User-Agent parsing, and the [`IExceptionMapper`](/asp-net-core/exceptions) contract this package's exceptions map through.
- `AlmightyShogun.AspNet.Localization` &mdash; resolves the message on each failure from the `auth` and `passwords` files described in [Localization](./localization).
- `AlmightyShogun.AspNet.RequestValidation` &mdash; supplies the `[Required]`, `[Email]`, `[Min]`, and `[PasswordSecure]` rules carried by the [request models](./requests/login-request).

## Startup Registration

[`AddAuth`](/asp-net-auth/extensions/add-auth) binds the token settings and the app resolver that credential flows read; [`AddAuthCredentials`](./extensions/add-auth-credentials) maps your context onto the package base context and registers the credential services against your user entity.

::: warning
Register JWT auth first. Credential auth resolves [`AuthSettings`](/asp-net-auth/configuration) and [`IAppHostResolver`](/asp-net-auth/services/app-host-resolver) at construction, so a container missing them fails when the first credential service is resolved, not at startup.
:::

::: code-group

```csharp [Program.cs]
using AlmightyShogun.AspNet.Core;
using AlmightyShogun.AspNet.Auth;
using Microsoft.EntityFrameworkCore;
using AlmightyShogun.AspNet.Localization;
using AlmightyShogun.AspNet.Auth.Credentials;
using AlmightyShogun.AspNet.RequestValidation;

builder.Services
    .AddMessageLocalization(builder.Configuration)
    .AddHttpErrorResponseWriter()
    .AddExceptionHandling()
    .AddAuth(builder.Configuration)
    .AddAspNetValidation()
    .AddDbContext<AppDbContext>(options => ...)
    .AddAuthCredentials<AppDbContext, AppUser>(builder.Configuration);

WebApplication app = builder.Build();

app.UseHttpErrorResponses();
app.UseAspNetValidation();
```

```csharp [AppDbContext.cs]
using Microsoft.EntityFrameworkCore;
using AlmightyShogun.AspNet.Auth.Credentials;

public sealed class AppDbContext(
    DbContextOptions<AppDbContext> options
) : AuthDbContext<AppUser>(options);
```

```csharp [AppUser.cs]
using AlmightyShogun.AspNet.Auth.Credentials;

public sealed class AppUser : AuthUser;
```

:::

## Data protection keys

Two-factor secrets are encrypted with ASP.NET Core data protection before they are stored. The default key ring lives on the local machine, so an application running on more than one host, or in a container without a persisted key directory, must configure a shared key store before anyone enrols. Losing the keys makes every stored secret unreadable and forces every enrolled user to set up their authenticator again.
