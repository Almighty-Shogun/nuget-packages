---
params:
    - name: configuration
      description: Application configuration containing the `Auth` section.
      type: IConfiguration

returns: The `IServiceCollection` instance with JWT authentication and authorization services configured.
---

# AddJwtAuth

Registers the complete ASP.NET JWT Auth service setup for an API. This is the package startup method: it registers and validates [`AuthSettings`](../configuration/auth-settings), adds `IHttpContextAccessor`, enables authorization, configures JWT bearer authentication, and registers the host resolver, app-audience authorization, and permission authorization services.

When `Auth.Hosts` contains mappings, the method enables JWT audience validation against the configured host app audiences and makes protected endpoints app-scoped by decorating default, fallback, custom, and generated permission policies with the app-audience requirement. When `Auth.Hosts` is empty, audience and host/app validation are disabled.

`AddJwtAuth` can register without ASP.NET Utils error-response services, but JWT Auth uses [`HttpErrorException`](/asp-net-utils/types/http-error-exception) for required refresh-token reads, current-user id reads, and unresolved host/app access. Register [`AddHttpErrorResponses`](/asp-net-utils/extensions/add-http-error-responses) before `AddJwtAuth` and add [`UseHttpErrorResponses`](/asp-net-utils/extensions/use-http-error-responses) to the request pipeline when those failures should be converted into standardized JSON error bodies.

## Usage

::: warning
Requires an `Auth` section in application configuration, usually from `appsettings.json`.
:::

```csharp
using AlmightyShogun.AspNet.Utils;
using AlmightyShogun.AspNet.JwtAuth;

builder.Services
    .AddHttpErrorResponses(builder.Configuration)
    .AddJwtAuth(builder.Configuration);

app.UseHttpErrorResponses();
```

<FrontmatterDocs/>

## Type signature

```csharp
public IServiceCollection AddJwtAuth(
    IConfiguration configuration
);
```
