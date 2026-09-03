---
params:
    - name: configuration
      description: Application configuration containing the `Auth` section.
      type: IConfiguration

    - name: registerExceptionHandler
      description: Whether to register the handler that turns this package's exceptions into standardized error responses. Pass `false` where the application already has an exception handler covering them.
      type: bool
      default: 'true'

returns: The `IServiceCollection` instance with JWT authentication and authorization services configured.
---

# AddJwtAuth

Registers everything the package needs: the bound [`AuthSettings`](../configuration), JWT bearer authentication, the host resolver, and the app-audience and permission authorization services. It also registers a mapper covering this package's [exceptions](../exceptions), so each becomes a standardized error response.

A non-empty [`Hosts`](../configuration) mapping turns on audience validation and decorates every policy, generated or declared, with the app-audience requirement.

## Usage

::: warning
Requires an `Auth` section in application configuration, usually from `appsettings.json`.
:::

```csharp
using AlmightyShogun.AspNet.Core;
using AlmightyShogun.AspNet.JwtAuth;
using AlmightyShogun.AspNet.Localization;

builder.Services
    .AddMessageLocalization(builder.Configuration)
    .AddHttpErrorResponseWriter()
    .AddExceptionHandling()
    .AddJwtAuth(builder.Configuration);

WebApplication app = builder.Build();

app.UseHttpErrorResponses();
```

<FrontmatterDocs/>

## Type signature

```csharp
public IServiceCollection AddJwtAuth(
    IConfiguration configuration,
    bool registerExceptionHandler = true
);
```
