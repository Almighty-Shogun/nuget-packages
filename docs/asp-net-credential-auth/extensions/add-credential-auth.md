---
params:
    - name: configuration
      description: The application configuration the optional `CredentialAuth` section is bound from. Pass `builder.Configuration`.
      type: IConfiguration

    - name: registerExceptionHandler
      description: Whether to add the handler that turns the package's exceptions into standardized HTTP errors. Pass `false` to answer them with a handler of your own; the mapper stays registered either way.
      type: bool
      default: 'true'

returns: The `IServiceCollection` instance with the credential authentication services registered.
---

# AddCredentialAuth

Registers [`IAuthUserService<TUser>`](../services/auth-user-service), [`IAuthSessionService<TUser>`](../services/auth-session-service), [`IAuthPasswordService`](../services/auth-password-service), and [`IAuthTwoFactorService<TUser>`](../services/auth-two-factor-service) as scoped services, alongside an [`AuthDbContext<TUser>`](../types/auth-db-context) resolving to the same `TDbContext` the request already holds, so a credential write joins whatever transaction the application has open.

Call it after [`AddAuth`](/asp-net-auth/extensions/add-auth). Credential flows read [`AuthSettings`](/asp-net-auth/configuration), resolve the current application through [`IAppHostResolver`](/asp-net-auth/services/app-host-resolver), and mint access tokens through the JWT package's generator, so the same issuer, secret, lifetime, and audience rules apply to both.

## Usage

```csharp
using AlmightyShogun.AspNet.Core;
using Microsoft.EntityFrameworkCore;
using AlmightyShogun.AspNet.Auth;
using AlmightyShogun.AspNet.Localization;
using AlmightyShogun.AspNet.CredentialAuth;
using AlmightyShogun.AspNet.RequestValidation;

builder.Services
    .AddMessageLocalization(builder.Configuration)
    .AddHttpErrorResponseWriter()
    .AddExceptionHandling()
    .AddAuth(builder.Configuration)
    .AddAspNetValidation()
    .AddDbContext<AppDbContext>(options => ...)
    .AddCredentialAuth<AppDbContext, AppUser>(builder.Configuration);
```

<FrontmatterDocs/>

## Type signature

```csharp
public IServiceCollection AddCredentialAuth<TDbContext, TUser>(
    IConfiguration configuration,
    bool registerExceptionHandler = true
) where TDbContext : AuthDbContext<TUser> where TUser : AuthUser;
```
