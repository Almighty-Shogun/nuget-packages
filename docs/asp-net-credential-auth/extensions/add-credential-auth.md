---
returns: The `IServiceCollection` instance with credential authentication services configured.
---

# AddCredentialAuth

Registers the services used by ASP.NET Credential Auth for a specific EF Core authentication context and user entity. The method maps the application DbContext to [`AuthDbContext<TUser>`](../types/auth-db-context), registers the internal implementation behind the public service contracts, and registers the credential-specific validation rules used by the package attributes.

Call this method after [`AddJwtAuth`](/asp-net-jwt-auth/extensions/add-jwt-auth) because credential auth reads [`AuthSettings`](/asp-net-jwt-auth/configuration/auth-settings), resolves the current app through [`IAppHostResolver`](/asp-net-jwt-auth/services/app-host-resolver), and creates JWT access tokens with the same issuer, secret, lifetime, and app audience rules.

## Usage

```csharp
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

<FrontmatterDocs/>

## Type signature

```csharp
public IServiceCollection AddCredentialAuth<TDbContext, TUser>()
    where TDbContext : AuthDbContext<TUser>
    where TUser : AuthUser;
```
