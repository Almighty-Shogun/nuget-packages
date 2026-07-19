# Installation

Install `AlmightyShogun.AspNet.JwtAuth` in the ASP.NET Core API that needs to validate JWT access tokens and register the package's permission and app-audience authorization services. The package targets `net10.0`, uses ASP.NET Core framework APIs, and expects an `Auth` configuration section to be available when authentication is registered.

```sh
dotnet add package AlmightyShogun.AspNet.JwtAuth
```

## Dependencies

### Framework references

- `Microsoft.AspNetCore.App` &mdash; provides the ASP.NET Core authentication, authorization, HTTP, controller, and middleware infrastructure used by the package.

### Package references

- `Microsoft.AspNetCore.Authentication.JwtBearer` `10.0.10` &mdash; provides JWT bearer authentication middleware and token validation options.
- `Microsoft.IdentityModel.Tokens` `8.19.2` &mdash; provides token validation parameters and symmetric signing key types used directly by JWT setup.
- `System.IdentityModel.Tokens.Jwt` `8.19.2` &mdash; provides JWT claim name constants used by app-audience authorization.

### Project references

- `AlmightyShogun.AspNet.Utils` &mdash; provides [`HttpErrorException`](/asp-net-utils/types/http-error-exception) and the optional standardized HTTP error response pipeline used by JWT Auth failure paths.
- `AlmightyShogun.Utils` &mdash; provides the configuration binding helper used when registering [`AuthSettings`](./configuration/auth-settings).

## Startup Registration

Register the package once while configuring application services. [`AddJwtAuth`](./extensions/add-jwt-auth) configures JWT bearer authentication, authorization services, `IHttpContextAccessor`, host-to-application resolution, app-audience authorization, refresh-token support, and the dynamic permission policy provider.

JWT Auth helpers and host/app resolution can throw [`HttpErrorException`](/asp-net-utils/types/http-error-exception) for authentication and authorization failures. Register ASP.NET Utils error responses before [`AddJwtAuth`](./extensions/add-jwt-auth) and add [`UseHttpErrorResponses`](/asp-net-utils/extensions/use-http-error-responses) to the request pipeline when those failures should be returned as standardized JSON error responses.

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
