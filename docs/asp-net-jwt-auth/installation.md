# Installation

Install `AlmightyShogun.AspNet.JwtAuth` in the ASP.NET Core API that needs to validate JWT access tokens and register the package's permission and app-audience authorization services. The package targets `net10.0`, uses ASP.NET Core framework APIs, and expects an `Auth` configuration section to be available when authentication is registered.

```sh
dotnet add package AlmightyShogun.AspNet.JwtAuth
```

## Dependencies

### Framework references

- `Microsoft.AspNetCore.App` &mdash; provides the ASP.NET Core authentication, authorization, HTTP, controller, and middleware infrastructure used by the package.

### Package references

- `Microsoft.AspNetCore.Authentication.JwtBearer` `10.0.11` &mdash; provides JWT bearer authentication middleware and token validation options.
- `Microsoft.IdentityModel.Tokens` `8.22.0` &mdash; provides token validation parameters and symmetric signing key types used directly by JWT setup.
- `System.IdentityModel.Tokens.Jwt` `8.22.0` &mdash; provides JWT claim name constants used by app-audience authorization.

### Project references

- `AlmightyShogun.Core` &mdash; provides the validated configuration binding helper the `Auth` section is bound with.
- `AlmightyShogun.AspNet.Core` &mdash; provides the [`IExceptionMapper`](/asp-net-core/exceptions) contract and the standardized HTTP error response pipeline this package's failures are answered through.
- `AlmightyShogun.AspNet.Localization` &mdash; resolves the message shown for each mapped failure, so a rejected request is explained in the caller's language.

## Startup Registration

Register the package once while configuring application services. [`AddJwtAuth`](./extensions/add-jwt-auth) configures JWT bearer authentication, authorization services, `IHttpContextAccessor`, host-to-application resolution, app-audience authorization, refresh-token support, and the dynamic permission policy provider.

The helpers and host resolution throw the package's own [exceptions](./exceptions), which [`AddJwtAuth`](./extensions/add-jwt-auth) maps to standardized responses. Add [`UseHttpErrorResponses`](/asp-net-core/extensions/use-http-error-responses) to the pipeline for those responses to reach the client as JSON.

::: warning
Requires an `Auth` section in application configuration, usually from `appsettings.json`.
:::

```csharp
using AlmightyShogun.AspNet.Core;
using AlmightyShogun.AspNet.JwtAuth;

builder.Services
    .AddHttpErrorResponses(builder.Configuration)
    .AddJwtAuth(builder.Configuration);

app.UseHttpErrorResponses();
```
