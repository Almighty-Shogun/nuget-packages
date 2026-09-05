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

# AddAuth

Registers everything the package needs: the bound [`AuthSettings`](../configuration), JWT bearer authentication, the host resolver, the token generator, and the app-audience and permission authorization services. It also registers a mapper covering this package's [exceptions](../exceptions), so each becomes a standardized error response.

Every policy, generated or declared, carries the app-audience requirement, which checks a token's audience against the app the request host resolves to whenever a non-empty [`Hosts`](../configuration) mapping is configured.

## Usage

```csharp
using AlmightyShogun.AspNet.Core;
using AlmightyShogun.AspNet.Auth;
using AlmightyShogun.AspNet.Localization;

builder.Services
    .AddMessageLocalization(builder.Configuration)
    .AddHttpErrorResponseWriter()
    .AddExceptionHandling()
    .AddAuth(builder.Configuration);

WebApplication app = builder.Build();

app.UseHttpErrorResponses();
```

<FrontmatterDocs/>

## Type signature

```csharp
public IServiceCollection AddAuth(
    IConfiguration configuration,
    bool registerExceptionHandler = true
);
```
