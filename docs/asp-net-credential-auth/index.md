# ASP.NET Credential Auth

Adds username or email password authentication to ASP.NET Core APIs using Entity Framework Core storage, JWT access tokens, refresh-token sessions, password reset tokens, and validation attributes for common credential flows.

Use this package when an API needs first-party account login instead of an external provider such as Discord or Google. The package stores users, refresh sessions, and password reset tokens in an application EF Core context, then uses [ASP.NET JWT Auth](/asp-net-jwt-auth/) to issue app-scoped access tokens and [ASP.NET Validation](/asp-net-validation/) to validate credential request DTOs.

The package targets `net10.0`, uses the ASP.NET Core shared framework, and depends on [ASP.NET Utils](/asp-net-utils/) for session metadata and standardized HTTP errors.

## Categories

- [Localization](./localization) &mdash; message files used by login, password, and credential validation failures.
- [Controllers](./controllers) &mdash; route examples for login, registration, refresh, logout, and forgot-password flows.
- [Extensions](./extensions/add-credential-auth) &mdash; service registration for the credential auth services and custom validation rules.
- [Attributes](./attributes/current-password-attribute) &mdash; request validation attributes for credential-specific rules.
- [Services](./services/auth-user-service) &mdash; dependency-injection contracts for login, registration, password, session, and token operations.
- [Requests](./requests/login-request) &mdash; reusable request DTOs for login, registration, and password reset flows.
- [Results](./results/auth-session-result) &mdash; response model returned when a credential flow creates or refreshes a session.
- [Types](./types/auth-db-context) &mdash; EF Core base context, user entity, refresh sessions, and password reset token entities.

## Quick Example

::: code-group

```csharp [Program.cs]
using AlmightyShogun.AspNet.Utils;
using Microsoft.EntityFrameworkCore;
using AlmightyShogun.AspNet.JwtAuth;
using AlmightyShogun.AspNet.Validation;
using AlmightyShogun.AspNet.CredentialAuth;

builder.Services
    .AddHttpErrorResponses(builder.Configuration)
    .AddJwtAuth(builder.Configuration)
    .AddAspNetValidation()
    .AddDbContext<AppDbContext>(options =>
        options.UseSqlite(builder.Configuration.GetConnectionString("Database")))
    .AddCredentialAuth<AppDbContext, AppUser>();
```

```csharp [AppDbContext.cs]
using Microsoft.EntityFrameworkCore;
using AlmightyShogun.AspNet.CredentialAuth;

public sealed class AppDbContext(DbContextOptions<AppDbContext> options)
    : AuthDbContext<AppUser>(options);
```

```csharp [AppUser.cs]
using AlmightyShogun.AspNet.CredentialAuth;

public sealed class AppUser : AuthUser
{
    public string DisplayName { get; set; } = string.Empty;
}
```

:::
