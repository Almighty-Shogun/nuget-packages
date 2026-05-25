# Installation

Install `AlmightyShogun.AspNet.JwtAuth` in the ASP.NET Core API that needs to validate JWT access tokens and register the package's permission authorization services. The package targets `net10.0`, uses ASP.NET Core framework APIs, and expects an `Auth` configuration section to be available when authentication is registered.

```sh
dotnet add package AlmightyShogun.AspNet.JwtAuth
```

## Dependencies

- `Microsoft.AspNetCore.App` framework reference &mdash; provides the ASP.NET Core authentication, authorization, HTTP, controller, and middleware infrastructure used by the package.
- `Microsoft.AspNetCore.Authentication.JwtBearer` `10.0.8` &mdash; provides JWT bearer authentication middleware and token validation options.
- `AlmightyShogun.Utils` project reference &mdash; provides the configuration binding helper used when registering `AuthSettings`.

## Startup Registration

Register the package once while configuring application services. `AddApiAuth` also configures JWT bearer authentication, authorization services, `IHttpContextAccessor`, host-to-application resolution, and the dynamic permission policy provider.

::: warning
Requires an `Auth` section in application configuration, usually from `appsettings.json`.
:::

```csharp
using AlmightyShogun.AspNet.JwtAuth;

builder.Services.AddApiAuth(builder.Configuration);
```
