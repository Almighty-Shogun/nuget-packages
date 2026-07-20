# Installation

Install `AlmightyShogun.AspNet.CredentialAuth` in the ASP.NET Core API that owns credential-based users, sessions, and password reset tokens. The package expects the application to register an EF Core context derived from [`AuthDbContext<TUser>`](./types/auth-db-context) and to configure JWT auth before registering credential auth.

```sh
dotnet add package AlmightyShogun.AspNet.CredentialAuth
```

## Dependencies

### Framework references

- `Microsoft.AspNetCore.App` &mdash; provides ASP.NET Core HTTP, dependency injection, Identity password hashing, and WebUtilities APIs used by the package.

### Package references

- `Microsoft.EntityFrameworkCore` `10.0.10` &mdash; provides the base DbContext, DbSet, model-building, and query APIs used for users, sessions, and reset tokens.

### Project references

- `AlmightyShogun.AspNet.JwtAuth` &mdash; provides [`AuthSettings`](/asp-net-jwt-auth/configuration/auth-settings), [`IAppHostResolver`](/asp-net-jwt-auth/services/app-host-resolver), JWT token dependencies, and refresh-token cookie helpers used by credential flows.
- `AlmightyShogun.AspNet.Utils` &mdash; provides [`SessionContext`](/asp-net-utils/records/session-context), User-Agent parsing, and [`HttpErrorException`](/asp-net-utils/types/http-error-exception).
- `AlmightyShogun.AspNet.Validation` &mdash; provides the validation attributes and custom rule infrastructure used by credential request DTOs.

## Startup Registration

Register ASP.NET Utils, JWT Auth, and ASP.NET Validation before credential auth. [`AddCredentialAuth`](./extensions/add-credential-auth) registers the credential services, maps your application DbContext to the package base context, and registers the credential-specific validation rules.

::: warning
Requires an `Auth` section in application configuration through [ASP.NET JWT Auth](/asp-net-jwt-auth/configuration), usually from `appsettings.json`.
:::

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

WebApplication app = builder.Build();

app.UseHttpErrorResponses();
app.UseAspNetValidation();
```

```csharp [AppDbContext.cs]
using Microsoft.EntityFrameworkCore;
using AlmightyShogun.AspNet.CredentialAuth;

public sealed class AppDbContext(DbContextOptions<AppDbContext> options)
    : AuthDbContext<AppUser>(options);
```

```csharp [AppUser.cs]
using AlmightyShogun.AspNet.CredentialAuth;

public sealed class AppUser : AuthUser;
```

:::
