---
outline: deep

params:
    - name: configuration
      description: Application configuration containing the `Auth` section.
      type: IConfiguration

returns: The `IServiceCollection` instance with authentication and authorization services configured.
---

# AddApiAuth

Registers the complete ASP.NET JWT Auth service setup for an API. This is the main startup method for the package: it binds `AuthSettings`, adds `IHttpContextAccessor`, enables authorization, registers JWT bearer authentication, and registers the host resolver and permission authorization services used by `AuthPermissionAttribute`.

Use this method when the application wants the package's full behavior. If an application only wants to add JWT bearer authentication to an existing authentication setup, use `AddJwtBearerAuthentication` directly instead.

## Usage

::: warning
Requires an `Auth` section in application configuration, usually from `appsettings.json`.
:::

```csharp
using AlmightyShogun.AspNet.JwtAuth;

builder.Services.AddApiAuth(builder.Configuration);
```

<FrontmatterDocs/>

## Type signature

```csharp
public IServiceCollection AddApiAuth(
    IConfiguration configuration
);
```

## Uses

- [AddJwtBearerAuthentication](./add-jwt-bearer-authentication)
- [AuthSettings](../configuration/auth-settings)
- [AppHostResolver](../classes/app-host-resolver/)
